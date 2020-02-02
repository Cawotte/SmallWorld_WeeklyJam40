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
    private Bridge currentBridge = null;

    private Tilemap bridgeTilemap;

    private Grid grid
    {
        get
        {
            return bridgeTilemap.layoutGrid;
        }
    }

    public Bridge CurrentBridge { get => currentBridge; }
    public Tilemap BridgeTilemap { set => bridgeTilemap = value; }

    public void StartNewBridge(Vector3 worldPos)
    {
        /*
        currentBridge
        if (HasBridge(worldPos) || currentBridge != null)
        {
            //Already has a bridge
            return false;
        } */

        currentBridge = new Bridge();
        bridges.Add(currentBridge);
        AddPlank(worldPos);

    }

    public void ExitCurrentBridge()
    {
        currentBridge = null;
    }

    public void AddPlank(Vector3 worldPos)
    {
        AddPlank(currentBridge, worldPos);
    }


    public void RemovePlank(Vector3 worldPos)
    {
        RemovePlank(currentBridge, worldPos);
    }

    public void SetTargetBridgeAsCurrent(Vector3 worldPos)
    {
        currentBridge = GetBridgeAt(worldPos);
    }

    public bool ArePlanksOnSameBridge(Vector3 worldPos1, Vector3 worldPos2)
    {
        Bridge bridge = GetBridgeAt(worldPos1);

        if (bridge != null)
        {
            return bridge.Equals(GetBridgeAt(worldPos2));
        }

        return false;
    }

    #region Private Methods

    private void AddPlank(Bridge bridge, Vector3 worldPos)
    {
        bridge.AddPlank(WorldToCell(worldPos));
        RenderBridge(bridge);
    }


    private void RemovePlank(Bridge bridge, Vector3 worldPos)
    {
        Vector3Int plankCell = WorldToCell(worldPos);

        bridge.RemovePlank(plankCell);

        bridgeTilemap.SetTile(plankCell, null); //set tile empty
        RenderBridge(bridge);

        if (bridge.IsEmpty)
        {
            bridges.Remove(bridge);
        }
    }


    private Bridge GetBridgeAt(Vector3 worldPos)
    {
        foreach (Bridge bridge in bridges)
        {

            //TODO : BETTER
            if (bridge.Contains(WorldToCell(worldPos)))
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

    #endregion
}
