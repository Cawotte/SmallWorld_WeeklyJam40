using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Bridge {


    public Tilemap bridgesTilemap;
    //public Sprite bridgeSprite; //= (Sprite)Resources.Load("Sprites/basictiles_8");
    //public RuleTile tileBridge;
    public BridgeTiles bridgetiles; 

    private bool hasEnd = false;
    [SerializeField] private List<Vector2> plankCoords = new List<Vector2>();


    /*  Bridge rules :
     *  - You can only add Planks at the end of the bridge.
     *  - You can remove planks by walking back the bridge when constructed, or crossing it again in any sense.
     *  
     * 
     * */
     
    //Constructors
    public Bridge(Tilemap bridgesTilemap, BridgeTiles bridgeTiles)
    {
        this.bridgesTilemap = bridgesTilemap;
        //this.tileBridge = tileBridge;
        this.bridgetiles = bridgeTiles;
    }

        /*
    public Bridge InitB(Tilemap bridgesTilemap)
    {
        this.bridgesTilemap = bridgesTilemap;
        plankCoords = new List<Vector2>();

        return this;
    } */


    //Methods
    public void addPlank(Vector2 plankCoor)
    {
        //Tile bridgeTile = ScriptableObject.CreateInstance<Tile>();
        //bridgeTile.sprite = bridgeSprite;

        plankCoords.Add(plankCoor);

        /*Vector3Int cellCoor = bridgesTilemap.WorldToCell(plankCoor);
        bridgesTilemap.SetTile(cellCoor, tileBridge);
        //bridgesTilemap.RefreshTile(cellCoor);
        bridgesTilemap.RefreshAllTiles(); */

        bridgetiles.renderTiles(bridgesTilemap, plankCoords);

    }

    public void removeFirstPlank()
    {
        plankCoords.RemoveAt(0);
    }
    public void removeLastPlank()
    {
        if (hasEnd) hasEnd = false;

        Vector2 removedCoor = lastPlank();
        plankCoords.RemoveAt(plankCoords.Count - 1);

        Vector3Int cellCoor = bridgesTilemap.WorldToCell(removedCoor);
        bridgesTilemap.SetTile(cellCoor, null);
        bridgesTilemap.RefreshTile(cellCoor);
    }
    public void reverseBridge()
    {
        plankCoords.Reverse();
    }

    //Booleans

    //Return true if PlayerPos = lastPlank pos.
    public bool isOnLastPlank(Vector2 playerPos)
    {
        return lastPlank().Equals(playerPos);
    }

    public bool canRemovePlank(Vector2 playerPos, Vector2 plankPos)
    {
        //A plank can be removed if

        //it's the first plank and the bridge has an end.
        if ( plankPos.Equals(firstPlank()) && !hasEnd )
            return true;
        //it's the last plank and the player is leaving it.
        if (plankPos.Equals(lastPlank()) && playerPos.Equals(lastPlank()))
            return true;

        return false;
    }

    public bool isExtremity(Vector2 posCell)
    {
        return (posCell.Equals(lastPlank()) || posCell.Equals(firstPlank()));
    }

 
    //return true if the player is leaving the last plank for the one before the last.
    public bool playerIsBackTracking(Vector2 playerPos, Vector2 targetPos)
    {
        //If both coordinates belongs to the bridge
        if (contains(playerPos) && contains(targetPos))
        {
            //If there's only one plank left.
            if (plankCoords.Count == 1)
                return playerPos.Equals(lastPlank());
            //Else if the player is moving from last Plank to plank before last.
            return (playerPos.Equals(lastPlank()) && targetPos.Equals(beforeLastPlank()));
        }
        else
            return false;
    }

    public bool contains(Vector2 plankPos)
    {
        return plankCoords.Contains(plankPos);
    }
    
    public bool isEmpty()
    {
        return plankCoords.Count == 0;
    }

    //Static methods

    //Return the bridge in the given list containing the given position. If there is none, return null.
    public static Bridge belongsTo(List<Bridge> bridgeList, Vector2 PlankPos)
    {
        foreach (Bridge bridge in bridgeList)
        {
            foreach (Vector2 pos in bridge.plankCoords)
            {
                //Debug.Log("Pos =" + pos.ToString());
                if (Vector2.Distance(pos, PlankPos) <= float.Epsilon )
                    return bridge;
            }
            //if (bridge.contains(PlankPos))
               /// return bridge;
            
        }

        Debug.Log("Coor not found : " + PlankPos.ToString());

        return null;
    }

    //Getters - Setters
    public Vector2 lastPlank()
    {
        return plankCoords[plankCoords.Count - 1];
    }
    public Vector2 beforeLastPlank()
    {
        return plankCoords[plankCoords.Count - 2];
    }
    public Vector2 firstPlank()
    {
        return plankCoords[0];
    }
    public void setHasEnd(bool boolean)
    {
        hasEnd = boolean;
    }
    public bool hasAnEnd()
    {
        return hasEnd;
    }



}
