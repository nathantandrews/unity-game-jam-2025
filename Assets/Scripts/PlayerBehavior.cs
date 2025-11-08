using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private GameTimer gameTimer;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isAlive = true;
    private Tilemap groundTilemap;

    private Vector3Int lastCellPos;

    void Start()
    {
        // Get Rigidbody2D or add one
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        // Configure Rigidbody2D
        rb.freezeRotation = true;
        rb.gravityScale = 0f;

        gameTimer = FindFirstObjectByType<GameTimer>();

        groundTilemap = FindFirstObjectByType<Tilemap>();

        // Register initial position in TileOccupancyManager
        if (TileOccupancyManager.Instance != null && groundTilemap != null)
        {
            lastCellPos = groundTilemap.WorldToCell(transform.position);
            TileOccupancyManager.Instance.EnterTile(lastCellPos, gameObject);
        }
        else
        {
            Debug.LogWarning("TileOccupancyManager or Tilemap not found!");
        }
    }

    void Update()
    {
        if (!isAlive) return;

        // Player input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        // Move the player
        if (moveDirection.magnitude >= 0.1f)
            rb.linearVelocity = moveDirection * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;

        // Notify timer if moving
        if (gameTimer != null)
        {
            bool isMoving = moveDirection.magnitude >= 0.1f;
            gameTimer.SetPlayerMoving(isMoving);
        }

        // Track tile position
        UpdateTilePosition();
    }

    private void UpdateTilePosition()
    {
        if (TileOccupancyManager.Instance == null || groundTilemap == null)
            return;

        Vector3Int currentCell = groundTilemap.WorldToCell(transform.position);

        if (currentCell != lastCellPos)
        {
            TileOccupancyManager.Instance.LeaveTile(lastCellPos, gameObject);
            TileOccupancyManager.Instance.EnterTile(currentCell, gameObject);
            lastCellPos = currentCell;
        }
    }

    public void KillPlayer()
    {
        if (!isAlive) return;
        isAlive = false;

        Debug.Log("💀 Player Died!");
        rb.linearVelocity = Vector2.zero;

        // Disable player for now (you could add animation instead)
        gameObject.SetActive(false);
    }
}
