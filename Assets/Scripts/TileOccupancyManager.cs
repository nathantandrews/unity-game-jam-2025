using System.Diagnostics;
using UnityEngine;

public class TileOccupancyManager : MonoBehaviour
{
    public static TileOccupancyManager Instance;
    private Dictionary<Vector3Int, List<GameObject>> occupancy = new();

    void Awake()
    {
        Instance = this;
    }

    public void EnterTile(Vector3Int cellPos, GameObject entity)
    {
        if (!occupancy.ContainsKey(cellPos))
        {
            occupancy[cellPos] = new List<GameObject>();
        }
        occupancy[cellPos].Add(entity);
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
    }

    private void CheckCollision(Vector3Int cellPos)
    {
        if (!occupancy.TryGetValue(cellPos, out var entities))
        {
            return;
        }
        bool hasPlayer = false;
        bool hasAlly = false;
        bool hasEnemy = false;
        foreach (var e in entities)
        {
            if (e.CompareTag("Player"))
            {
                hasPlayer = true;
            }
            if (e.CompareTag("Ally"))
            {
                hasPlayer = true;
            }
            if (e.CompareTag("Enemy"))
            {
                hasEnemy = true;
            }
        }
        if (hasPlayer && hasEnemy)
        {
            Debug.Log($"Player and Enemy on same tile: {cellPos}");
            KillPlayer();
        }
        if (hasPlayer && hasAlly)
        {
            Debug.Log($"Player and Ally on same tile: {cellPos}");
        }
    }

    public void KillPlayer(GameObject player)
    {
        var behavior = player.GetComponent<PlayerScript>();
        if (behavior != null)
        {
            behavior.KillPlayer();
        }
        else
        {
            player.SetActive(false);
            Debug.Log("Player died (no PlayerScript found).");
        }
    }
}
