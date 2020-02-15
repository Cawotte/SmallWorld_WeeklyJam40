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
    public List<Bridge> Bridges { get => bridges; }

    public void StartNewBridge(Vector3 worldPos)
    {
        currentBridge = new Bridge();
        bridges.Add(currentBridge);
        AddPlank(currentBridge, worldPos);

    }

    public void ExitCurrentBridge()
    {
        currentBridge = null;
    }

    public void AddPlank(Vector3 worldPos, Vector3 worldPreviousPos)
    {
        //We use the previous pos to tell to which end add the planks

        //If the previous pos was not the last plank
        if (!WorldToCell(worldPreviousPos).Equals(currentBridge.LastPlank))
        {
            currentBridge.Reverse();
        }

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

    public bool IsAnExtremityOfCurrentBridge(Vector3 worldPos)
    {
        return currentBridge.IsAnExtremity(WorldToCell(worldPos));
    }

    public bool IsAnExtremityOfTargetBridge(Vector3 worldPos)
    {
        Bridge bridge = GetBridgeAt(worldPos);
        if (bridge != null)
        {
            return bridge.IsAnExtremity(WorldToCell(worldPos));
        }
        return false;
    }

    public bool ArePlanksConsecutive(Vector3 worldPos1, Vector3 worldPos2)
    {
        return currentBridge.ArePlanksConsecutive(WorldToCell(worldPos1), WorldToCell(worldPos2));
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
        //Verify we put the plank at the right end
        if (bridge.Size > 1 && Vector3Int.Distance(bridge.LastPlank, WorldToCell(worldPos)) > 1f)
        {
            bridge.Planks.Reverse();
        }
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

            if (bridge == currentBridge)
            {
                currentBridge = null;
            }
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
