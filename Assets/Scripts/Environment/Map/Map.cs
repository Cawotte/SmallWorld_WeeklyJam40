using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    private Grid grid;

    [SerializeField]
    private Tilemap groundTilemap = null;

    [SerializeField]
    private Tilemap obstaclesTilemap = null;

    [SerializeField]
    private Tilemap bridgesTilemap = null;

    [SerializeField]
    private BridgeBuilder bridgeBuilder = null;

    public BridgeBuilder BridgeBuilder { get => bridgeBuilder; }

    private void Awake()
    {
        grid = groundTilemap.layoutGrid;
        bridgeBuilder.BridgeTilemap = bridgesTilemap;
    }

    public bool IsGround(Vector3 worldPos)
    {
        return HasTile(groundTilemap, worldPos);
    }

    public bool IsObstacle(Vector3 worldPos)
    {
        return HasTile(obstaclesTilemap, worldPos);
    }

    public bool IsBridge(Vector3 worldPos)
    {
        return HasTile(bridgesTilemap, worldPos);
    }

    public bool IsWater(Vector3 worldPos)
    {
        return !IsGround(worldPos) && !IsObstacle(worldPos);
    }

    public bool CanMoveFromTo(Player player, Vector3 startPos, Vector3 endPos)
    {
        Vector3Int cellStartPos = grid.WorldToCell(startPos);
        Vector3Int cellEndPos = grid.WorldToCell(endPos);

        //Verify if the player can move to the target cell

        //Is it an adjacent cell ?
        if ((cellStartPos - cellStartPos).magnitude > 1f)
        {
            return false;
        }

        //Is it an obstacle ?
        if (IsObstacle(endPos))
        {
            return false;
        }


        // --- INTERACTABLE

        //test for obstacles objets (not belonging to tilemap)
        Interactable interactable = GetInteractable(endPos);

        //not null
        if (interactable)
        {
            //If can't move return, else continue because of possible bridge shenanigans
            if (!interactable.CanMoveTo(player))
            {
                return false;
            }
        }

        // --- BRIDGE

        bool playerCanBuildBridge = player.WoodCount > 0;
        bool playerIsOnGround = IsGround(startPos);

        bool endHasWater = IsWater(endPos);
        bool endHasBridge = IsBridge(endPos);

        // --- WALKING TO WATER TILE (With possible bridge on it)

        if (endHasWater)
        {
            //End pos is pure water
            if (!endHasBridge)
            {
                if (playerCanBuildBridge)
                {
                    //We can build a bridge to the next tile.

                    //Are we on the ground ?
                    if (playerIsOnGround)
                    {
                        //yes
                        //start a new bridge
                        bridgeBuilder.StartNewBridge(endPos); //also add plank
                    }
                    else
                    {
                        //no

                        //Are we on an extremity of the bridge ?
                        if (bridgeBuilder.IsAnExtremityOfCurrentBridge(startPos))
                        {
                            bridgeBuilder.AddPlank(endPos, startPos);
                        }
                        else
                        {
                            //Can't continue a bridge from mid-section of the bridge.
                            return false;
                        }
                    }

                    player.WoodCount--;

                    return true;
                }
                else
                {

                    //No wood to build the bridge
                    return false;
                }
            }
            else //Target Tile has a bridge
            {
                //Are we already on the same bridge ?
                if (bridgeBuilder.ArePlanksOnSameBridge(startPos, endPos))
                {
                    //yes

                    /**
                     * We can only backtrack on a bridge, one plank must be an extremity,
                     * and the other one the following one
                     * */

                    if (bridgeBuilder.IsAnExtremityOfCurrentBridge(startPos) &&
                        bridgeBuilder.ArePlanksConsecutive(startPos, endPos))
                    {
                        bridgeBuilder.RemovePlank(startPos); //remove behind ourselves.
                        player.WoodCount++;
                        return true;
                    }

                    return false;
                }
                else 
                {
                    //no

                    //Are we on ground and climbing on an extremity ?
                    if (playerIsOnGround && bridgeBuilder.IsAnExtremityOfTargetBridge(endPos))
                    {
                        //This is our new current bridge
                        bridgeBuilder.SetTargetBridgeAsCurrent(endPos);
                        
                        //Reverse the order of the bridge if we entered from the end
                        if (bridgeBuilder.CurrentBridge.LastPlank.Equals(grid.WorldToCell(endPos)))
                        {
                            bridgeBuilder.CurrentBridge.Reverse();
                        }

                        return true;
                    }
                    else
                    {
                        //movement from a bridge to a different one are forbidden
                        return false;
                    }


                }
            }
        }
        // --- WALKING TO GROUND FROM WATER
        else if (!playerIsOnGround) //if player on water, already tested target is ground
        {
            if (bridgeBuilder.CurrentBridge.Size == 1)
            {
                bridgeBuilder.RemovePlank(startPos);
                player.WoodCount++;
            }
            else
            {
                bridgeBuilder.ExitCurrentBridge();
            }

            return true;
        }

        // -- Simple Ground to Ground !

        return true;

    }


    private Interactable GetInteractable(Vector3 worldPos)
    {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(worldPos, worldPos);

        if (hit.collider == null)
        {
            return null;
        }
        return hit.collider.GetComponent<Interactable>();
    }

    private bool HasTile(Tilemap tilemap, Vector3 worldCellPos)
    {
        //Get Tile =/= null ?
        return tilemap.HasTile(tilemap.WorldToCell(worldCellPos));
    }

    private void OnDrawGizmos()
    {
        foreach (Bridge bridge in bridgeBuilder.Bridges)
        {
            if (!bridge.IsEmpty)
            {
                //Set color
                if (bridge == bridgeBuilder.CurrentBridge)
                {
                    Gizmos.color = Color.green;
                }
                else {
                    Gizmos.color = Color.cyan;
                }

                Vector3 offset = (Vector3.right + Vector3.up) * 0.5f;

                for (int i = 0; i < bridge.Size; i++)
                {
                    Vector3Int plank = bridge.Planks[i];
                    Gizmos.DrawSphere(grid.CellToWorld(plank) + offset, 0.15f);

                    if (bridge.Size > 1 && i < bridge.Size - 1)
                    {
                        Gizmos.DrawLine(grid.CellToWorld(plank) + offset, grid.CellToWorld(bridge.Planks[i + 1]) + offset);
                    }
                }


                Gizmos.color = Color.red;
                Gizmos.DrawSphere(grid.CellToWorld(bridge.Planks[0]) + offset, 0.15f);
            }

        }
    }

}
