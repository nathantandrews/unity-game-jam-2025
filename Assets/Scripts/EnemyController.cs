using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct EnemyMove
{
    public Vector2 direction;
    public float speed;
    public float duration;
}

public class EnemyController : MonoBehaviour
{
    public EnemyMove[] moves;
    public bool loop = true;
    
    private int currentMoveIndex = 0;
    private float moveTimer = 0f;
    private Vector3Int lastCellPos;

    [SerializeField] private Rigidbody2D rb;
    private Tilemap groundTilemap;
    private GameTimer gameTimer; 

    void Start()
    {
        gameTimer = FindFirstObjectByType<GameTimer>();
        groundTilemap = FindFirstObjectByType<Tilemap>();
        rb.freezeRotation = true;
    }
    void UpdateTilePosition()
    {
        Vector3Int currentCell = groundTilemap.WorldToCell(transform.position);
        if (currentCell != lastCellPos)
        {
            TileOccupancyManager.Instance.LeaveTile(lastCellPos, gameObject);
            TileOccupancyManager.Instance.EnterTile(currentCell, gameObject);
            lastCellPos = currentCell;
        }
    }

    void FixedUpdate()
    {
        if (moves.Length == 0) return;
        EnemyMove currentMove = moves[currentMoveIndex];
        moveTimer += Time.deltaTime;

        rb.MovePosition(rb.position + currentMove.direction.normalized * currentMove.speed * Time.deltaTime * gameTimer.currentSpeed);
        if (moveTimer >= currentMove.duration)
        {
            moveTimer = 0f;
            currentMoveIndex++;
            if (currentMoveIndex >= moves.Length)
            {
                if (loop)
                {
                    currentMoveIndex = 0;
                }
                else
                {
                    enabled = false;
                }
            }
        }
    }
}
