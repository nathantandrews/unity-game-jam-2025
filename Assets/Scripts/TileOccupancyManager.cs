using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOccupancyManager : MonoBehaviour
{
    public static TileOccupancyManager Instance;

    [Header("References")]
    public Tilemap tilemap;

    [Header("Tiles")]
    public TileBase defaultTile;
    public TileBase playerTile;
    public TileBase enemyTile;
    public TileBase allyTile;
    public TileBase goalTile;

    private GameManager gm;

    private Dictionary<Vector3Int, List<GameObject>> occupancy = new();
    private Dictionary<Vector3Int, TileBase> originalTiles = new();

    void Awake()
    {
        Instance = this;
    }

    public void EnterTile(Vector3Int cellPos, GameObject entity)
    {
        if (!occupancy.ContainsKey(cellPos))
            occupancy[cellPos] = new List<GameObject>();

        occupancy[cellPos].Add(entity);

        UpdateTileAppearance(cellPos);
        CheckCollision(cellPos);
    }

    public void LeaveTile(Vector3Int cellPos, GameObject entity)
    {
        if (occupancy.TryGetValue(cellPos, out var list))
        {
            list.Remove(entity);
            if (list.Count == 0)
            {
                occupancy.Remove(cellPos);
            }
        }

        UpdateTileAppearance(cellPos);
    }

    private void UpdateTileAppearance(Vector3Int cellPos)
    {
        if (tilemap == null) return;

        // If no entities, restore original tile
        if (!occupancy.TryGetValue(cellPos, out var entities) || entities.Count == 0)
        {
            if (originalTiles.TryGetValue(cellPos, out var originalTile))
            {
                tilemap.SetTile(cellPos, originalTile);
                originalTiles.Remove(cellPos); // optional, to keep dictionary clean
            }
            return;
        }

        // Store original tile if not already stored
        if (!originalTiles.ContainsKey(cellPos))
            originalTiles[cellPos] = tilemap.GetTile(cellPos);

        bool hasPlayer = false;
        bool hasEnemy = false;
        bool hasAlly = false;

        foreach (var e in entities)
        {
            if (e.CompareTag("Player")) hasPlayer = true;
            if (e.CompareTag("Enemy")) hasEnemy = true;
            if (e.CompareTag("Ally")) hasAlly = true;
        }

        // Priority for display
        if (hasEnemy) tilemap.SetTile(cellPos, enemyTile);
        else if (hasPlayer) tilemap.SetTile(cellPos, playerTile);
        else if (hasAlly) tilemap.SetTile(cellPos, allyTile);
    }

    private void CheckCollision(Vector3Int cellPos)
    {
        if (!occupancy.TryGetValue(cellPos, out var entities)) return;

        bool hasPlayer = false;
        bool hasEnemy = false;
        bool hasAlly = false;
        bool hasGoal = false;
        GameObject player = null;

        foreach (var e in entities)
        {
            if (e.CompareTag("Player"))
            {
                hasPlayer = true;
                player = e;
            }
            if (e.CompareTag("Enemy")) hasEnemy = true;
            if (e.CompareTag("Goal")) hasGoal = true;
            if (e.CompareTag("Ally")) hasAlly = true;
        }

        if (hasPlayer && hasEnemy && player != null)
        {
            Debug.Log($"Player and Enemy on same tile: {cellPos}");
            var behavior = player.GetComponent<PlayerBehavior>();
            behavior?.KillPlayer();
            gm?.EndLevel(false);
        }
        if (hasPlayer && hasGoal && player != null)
        {
            Debug.Log($"Player and Goal on same tile: {cellPos}");
            var gm = FindFirstObjectByType<GameManager>();
            gm?.EndLevel(true);
        }
        if (hasPlayer && hasAlly && player != null)
        {
            Debug.Log($"Player and Ally on same tile: {cellPos}");
            Debug.Log("Ally Benefits Not Implemented");
        }

    }
}
