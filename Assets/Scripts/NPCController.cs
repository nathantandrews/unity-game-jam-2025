using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Move
{
    public Vector2 direction;
    public float speed;
    public float duration;
}

public class NPCController : MonoBehaviour
{
    public Move[] moves;
    public bool loop = true, isAlive = true;

    private int currentMoveIndex = 0;
    private float moveTimer = 0f;
    private Vector3Int lastCellPos;

    [SerializeField] private Rigidbody2D rb;
    private Tilemap groundTilemap;
    private GameTimer gameTimer;

    void Start()
    {
        // ensure rb
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // ignore collision with player (optional)
        var npcCollider = GetComponent<Collider2D>();
        var player = GameObject.FindWithTag("Player");
        if (player != null && npcCollider != null)
        {
            var playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
                Physics2D.IgnoreCollision(npcCollider, playerCollider, true);
        }

        gameTimer = FindFirstObjectByType<GameTimer>();

        // use the same tilemap as the manager (safer)
        if (TileOccupancyManager.Instance != null)
            groundTilemap = TileOccupancyManager.Instance.tilemap;
        else
            groundTilemap = FindFirstObjectByType<Tilemap>();

        // initialize position and register once (so stationary NPCs appear)
        if (groundTilemap != null && TileOccupancyManager.Instance != null)
        {
            lastCellPos = groundTilemap.WorldToCell(transform.position);
            TileOccupancyManager.Instance.EnterTile(lastCellPos, gameObject);
        }
        else
        {
            Debug.LogWarning("NPCController: Tilemap or TileOccupancyManager not found at Start().");
        }

        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        // keep registration up-to-date even if not moving via physics (cheap check)
        UpdateTilePosition();
    }

    void UpdateTilePosition()
    {
        if (groundTilemap == null || TileOccupancyManager.Instance == null) return;

        Vector3Int currentCell = groundTilemap.WorldToCell(transform.position);

        // only update if cell actually changed
        if (currentCell != lastCellPos)
        {
            // leave old cell (only if valid)
            TileOccupancyManager.Instance.LeaveTile(lastCellPos, gameObject);

            // enter new cell
            TileOccupancyManager.Instance.EnterTile(currentCell, gameObject);

            lastCellPos = currentCell;
        }
    }

    void FixedUpdate()
    {
        if (moves.Length == 0) return;
        Move currentMove = moves[currentMoveIndex];
        moveTimer += Time.deltaTime;

        if (rb != null && gameTimer != null)
        {
            rb.MovePosition(rb.position + currentMove.direction.normalized * currentMove.speed * Time.deltaTime * gameTimer.currentSpeed);
        }
        else if (rb != null)
        {
            rb.MovePosition(rb.position + currentMove.direction.normalized * currentMove.speed * Time.deltaTime);
        }

        // Update tile after physics move (already called in Update too; harmless)
        UpdateTilePosition();

        if (moveTimer >= currentMove.duration)
        {
            moveTimer = 0f;
            currentMoveIndex++;
            if (currentMoveIndex >= moves.Length)
            {
                if (loop) currentMoveIndex = 0;
                else enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        // clean up occupancy if object destroyed
        if (TileOccupancyManager.Instance != null)
            TileOccupancyManager.Instance.LeaveTile(lastCellPos, gameObject);
    }

    public void KillNPC()
    {
        if (!isAlive) return;
        isAlive = false;

        Debug.Log($"NPC {gameObject.name} Died!");
        rb.linearVelocity = Vector2.zero;

        gameObject.SetActive(false);
    }
}
