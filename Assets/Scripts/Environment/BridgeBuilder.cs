using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BridgeBuilder
{
    [SerializeField]
    private BridgeTiles bridgeTiles;

    [SerializeField]
    private List<Bridge> bridges = new List<Bridge>();

    //Current bridge the player is on
    [SerializeField]
    private Bridge currentBridge;

    private Tilemap bridgeTilemap;

    private Grid grid
    {
        get
        {
            return bridgeTilemap.layoutGrid;
        }
    }

    public Bridge CurrentBridge { get => currentBridge; }

    public BridgeBuilder(Tilemap bridgeTilemap)
    {
        this.bridgeTilemap = bridgeTilemap;
    }

    public void ExitCurrentBridge()
    {

    }

    public bool StartNewBridge(Vector3 worldPos)
    {
        if (HasBridge(worldPos) || currentBridge != null)
        {
            //Already has a bridge
            return false;
        }

        currentBridge = new Bridge();
    }


    public void AddPlank(Vector3 worldPos)
    {

    }

    public void RemovePlank(Vector3 worldPos)
    {

    }

    private bool HasBridge(Vector3 worldPos)
    {
        return bridgeTilemap.HasTile(bridgeTilemap.layoutGrid.WorldToCell(worldPos));
    }

    private Bridge GetBridgeAt(Vector3 worldPos)
    {
        foreach (Bridge bridge in bridges)
        {

            //TODO : BETTER
            if (bridge.Contains(worldPos))
            {
                return bridge;
            }

        }

        //Not found

        return null;
    }

    private Vector3Int WorldToCell(Vector3 worldPos)
    {
        return bridgeTilemap.layoutGrid.WorldToCell(worldPos);
    }

    private void RenderBridge(Bridge bridge)
    {
        bridgeTiles.RenderBridge(bridgeTilemap, bridge);
    }
}
