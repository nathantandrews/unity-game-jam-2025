using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileOccupancyManager : MonoBehaviour
{
    public static TileOccupancyManager Instance;

    [Header("References")]
    public Tilemap tilemap; // Assign in the Inspector

    private Dictionary<Vector3Int, List<GameObject>> occupancy = new();

    void Awake()
    {
        Instance = this;

        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
    }

    public void EnterTile(Vector3Int cellPos, GameObject entity)
    {
        if (!occupancy.ContainsKey(cellPos))
        {
            occupancy[cellPos] = new List<GameObject>();
        }

        occupancy[cellPos].Add(entity);

        UpdateTileColor(cellPos);
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

        UpdateTileColor(cellPos);
    }

    private void UpdateTileColor(Vector3Int cellPos)
    {
        if (tilemap == null) return;

        Color color = Color.white;

        if (occupancy.TryGetValue(cellPos, out var entities))
        {
            bool hasPlayer = false;
            bool hasEnemy = false;
            bool hasAlly = false;
            bool hasGoal = false;

            foreach (var e in entities)
            {
                if (e.CompareTag("Player")) hasPlayer = true;
                if (e.CompareTag("Enemy")) hasEnemy = true;
                if (e.CompareTag("Ally")) hasAlly = true;
                if (e.CompareTag("Goal")) hasGoal = true;
            }

            // Color priority: Red if Enemy, Blue if Player
            if (hasEnemy) color = Color.red;
            else if (hasPlayer) color = Color.cyan;
            else if (hasAlly) color = Color.green;
            else if (hasGoal) color = Color.gold;
        }

        tilemap.SetColor(cellPos, color);
    }

    private void CheckCollision(Vector3Int cellPos)
    {
        if (!occupancy.TryGetValue(cellPos, out var entities))
            return;

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
            if (e.CompareTag("Ally")) hasAlly = true;
            if (e.CompareTag("Goal")) hasGoal = true;
        }

        if (hasPlayer && hasEnemy)
        {
            Debug.Log($"Player and Enemy on same tile: {cellPos}");
            KillPlayer(player);
        }

        if (hasPlayer && hasGoal)
        {
            Debug.Log($"Player reached the goal: {cellPos}");
        }

        if (hasPlayer && hasAlly)
        {
            Debug.Log($"Player and Ally on same tile: {cellPos}");
        }
    }

    public void KillPlayer(GameObject player)
    {
        var behavior = player.GetComponent<PlayerBehavior>();
        if (behavior != null)
        {
            behavior.KillPlayer();
        }
        else
        {
            player.SetActive(false);
            Debug.Log("Player died (no PlayerBehavior found).");
        }
    }
}
