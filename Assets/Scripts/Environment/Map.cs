using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tilemap groundTilemap;

    [SerializeField]
    private Tilemap obstaclesTilemap;

    [SerializeField]
    private Tilemap bridgesTilemap;

    public bool IsGround(Vector3 worldCellPos)
    {
        return HasTile(groundTilemap, worldCellPos);
    }

    public bool IsObstacle(Vector3 worldCellPos)
    {
        return HasTile(obstaclesTilemap, worldCellPos);
    }

    public bool IsBridge(Vector3 worldCellPos)
    {
        return HasTile(bridgesTilemap, worldCellPos);
    }

    private bool HasTile(Tilemap tilemap, Vector3 worldCellPos)
    {
        //Get Tile =/= null ?
        return tilemap.HasTile(tilemap.WorldToCell(worldCellPos));
    }
}
