using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOccupancyManager : MonoBehaviour
{
    public static TileOccupancyManager Instance;

    [Header("Core refs")]
    public Tilemap tilemap;

    [Header("Tile assets (assign in Inspector)")]
    public TileBase defaultTile;
    public TileBase playerTile;
    public TileBase enemyTile;
    public TileBase allyTile;
    public TileBase goalTile;

    // occupancy data
    private Dictionary<Vector3Int, List<GameObject>> occupancy = new();
    private Dictionary<Vector3Int, TileBase> originalTiles = new();

    void Awake()
    {
        Instance = this;
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
        Debug.Log($"[TOM] Awake. tilemap={(tilemap!=null?tilemap.name:"NULL")}");
    }

    public void EnterTile(Vector3Int cellPos, GameObject entity)
    {
        if (tilemap == null) { Debug.LogWarning("[TOM] EnterTile called but tilemap is NULL"); return; }

        if (!occupancy.ContainsKey(cellPos))
            occupancy[cellPos] = new List<GameObject>();

        if (!occupancy[cellPos].Contains(entity))
            occupancy[cellPos].Add(entity);

        Debug.Log($"[TOM] EnterTile {cellPos} by {entity.name} (tag={entity.tag}) - occupants={occupancy[cellPos].Count}");
        UpdateTileAppearance(cellPos);
        CheckCollision(cellPos);
    }

    public void LeaveTile(Vector3Int cellPos, GameObject entity)
    {
        if (!occupancy.TryGetValue(cellPos, out var list))
        {
            // nothing to remove, but log for debugging
            Debug.Log($"[TOM] LeaveTile called for {cellPos} but no list exists (entity={entity.name})");
            return;
        }

        if (list.Remove(entity))
            Debug.Log($"[TOM] LeaveTile {cellPos} removed {entity.name} remaining={list.Count}");
        else
            Debug.Log($"[TOM] LeaveTile {cellPos} did NOT remove {entity.name} (not found)");

        if (list.Count == 0)
            occupancy.Remove(cellPos);

        UpdateTileAppearance(cellPos);
    }

    private void UpdateTileAppearance(Vector3Int cellPos)
    {
        if (tilemap == null) return;

        // If empty -> restore original tile (if stored)
        if (!occupancy.TryGetValue(cellPos, out var entities) || entities.Count == 0)
        {
            if (originalTiles.TryGetValue(cellPos, out var orig))
            {
                tilemap.SetTileFlags(cellPos, TileFlags.None);
                tilemap.SetTile(cellPos, orig);
                originalTiles.Remove(cellPos);
                Debug.Log($"[TOM] Restored original tile at {cellPos}");
            }
            return;
        }

        // Save original if needed
        if (!originalTiles.ContainsKey(cellPos))
            originalTiles[cellPos] = tilemap.GetTile(cellPos);

        bool hasPlayer = false, hasEnemy = false, hasAlly = false, hasGoal = false;
        foreach (var e in entities)
        {
            if (e.CompareTag("Player")) hasPlayer = true;
            if (e.CompareTag("Enemy")) hasEnemy = true;
            if (e.CompareTag("Ally")) hasAlly = true;
            if (e.CompareTag("Goal")) hasGoal = true;
        }

        // Priority: Enemy > Player > Ally > Goal
        TileBase toSet = defaultTile;
        if (hasEnemy && enemyTile != null) toSet = enemyTile;
        else if (hasPlayer && playerTile != null) toSet = playerTile;
        else if (hasAlly && allyTile != null) toSet = allyTile;
        else if (hasGoal && goalTile != null) toSet = goalTile;

        tilemap.SetTileFlags(cellPos, TileFlags.None);
        tilemap.SetTile(cellPos, toSet);
        Debug.Log($"[TOM] Set tile at {cellPos} to {(toSet!=null?toSet.name:"NULL")} (P:{hasPlayer} E:{hasEnemy} A:{hasAlly})");
    }

    private void CheckCollision(Vector3Int cellPos)
    {
        if (!occupancy.TryGetValue(cellPos, out var entities)) return;

        bool hasPlayer = false, hasEnemy = false, hasAlly = false, hasGoal = false;
        GameObject player = null;
        GameObject ally = null;
        GameObject enemy = null;
        GameObject goal = null;

        foreach (var e in entities)
        {
            if (e.CompareTag("Player")) { hasPlayer = true; player = e; }
            if (e.CompareTag("Enemy")) { hasEnemy = true; enemy = e; }
            if (e.CompareTag("Ally")) { hasAlly = true; ally = e; }
            if (e.CompareTag("Goal")) { hasGoal = true; goal = e; }
        }

        if (hasPlayer && hasEnemy && player != null)
        {
            Debug.Log($"[TOM] Player collision at {cellPos} -> killing player");
            var pb = player.GetComponent<PlayerBehavior>();
            pb?.KillPlayer();
        }

        if (hasEnemy && hasAlly)
        {
            Debug.Log($"[TOM] Ally {ally.name} died at {cellPos} due to {enemy.name}");
            var al = ally.GetComponent<NPCController>();
            al?.KillNPC();
        }
        if (hasPlayer && hasGoal)
        {
            Debug.Log($"[TOM] Player reached Goal -> ending level");

        }
    }

    // debug helper (call from editor or other scripts)
    public void PrintOccupancy()
    {
        Debug.Log("[TOM] Occupancy dump:");
        foreach (var kv in occupancy)
        {
            var names = string.Join(", ", kv.Value.ConvertAll(g => g.name));
            Debug.Log($"  {kv.Key} => {names}");
        }
    }
}
