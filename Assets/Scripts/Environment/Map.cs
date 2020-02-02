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

    [SerializeField]
    private BridgeBuilder bridgeBuilder;

    public BridgeBuilder BridgeBuilder { get => bridgeBuilder; }

    private void Awake()
    {
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
            Debug.Log("Can't move, not adjacent tiles");
            return false;
        }

        //Is it an obstacle ?
        if (IsObstacle(endPos))
        {
            Debug.Log("Can't move, obstacle");
            return false;
        }

        /**
         *  We need to test two conditions, If the player can move to the next tile :
         *      - Considering bridges
         *      - Considering interactables
         *      
         *  Because the player can't move despite bridge if a door is locked, 
         *  and can't move despite an unlocked door if it can bridge over water.s
         */


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

        bool playerCanBuildBridge = player.woodCount > 0;
        bool playerIsOnGround = IsGround(startPos);

        bool endHasWater = IsWater(endPos);
        bool endHasBridge = IsBridge(endPos);

        // --- WALKING TO WATER TILE (With possible bridge on it)

        if (endHasWater)
        {
            //End pos is pure water
            if (!endHasBridge)
            {
                //No wood to build the bridge
                if (!playerCanBuildBridge)
                {
                    return false;
                }
                else
                {
                    //We can build a bridge to the next tile.

                    //Are we on the ground ?
                    if (playerIsOnGround)
                    {
                        //no
                        //start a new bridge
                        bridgeBuilder.StartNewBridge(endPos); //also add plank
                    }
                    else
                    {
                        //yes
                        //continue current bridge
                        bridgeBuilder.AddPlank(endPos);
                    }

                    player.woodCount--;

                    return true;
                }
            }
            else //Target Tile has a bridge
            {
                //Are we already on the same bridge ?
                if (bridgeBuilder.ArePlanksOnSameBridge(startPos, endPos))
                {
                    //yes
                    bridgeBuilder.RemovePlank(startPos); //remove behind ourselves.
                    player.woodCount++;

                    return true;
                }
                else 
                {
                    //no

                    //Are we on ground ?
                    if (playerIsOnGround)
                    {
                        //This is our new current bridge
                        bridgeBuilder.SetTargetBridgeAsCurrent(endPos);
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
                player.woodCount++;
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

}
