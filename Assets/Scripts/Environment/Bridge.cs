using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Bridge {


    private bool hasEnd = false;

    [SerializeField] private List<Vector3Int> planks = new List<Vector3Int>();


    /*  Bridge rules :
     *  - You can only add Planks at the end of the bridge.
     *  - You can remove planks by walking back the bridge when constructed, or crossing it again in any sense.
     *  
     * 
     * */

    //Properties

    public bool IsEmpty
    {
        get
        {
            return planks.Count == 0;
        }
    }

    public Vector3Int LastPlank
    {
        get
        {
            return planks[planks.Count - 1];
        }
    }
    public Vector3Int BeforeLastPlank
    {
        get
        {
            return planks[planks.Count - 2];
        }
    }

    public Vector3Int FirstPlank
    {
        get
        {
            return planks[0];
        }
    }

    public bool HasEnd
    {
        get; set;
    }
    public List<Vector3Int> Planks { get => planks; }

    //Constructors
    public Bridge()
    {
        this.planks = new List<Vector3Int>();
    }


    //Methods
    public void AddPlank(Vector3Int plankCoor)
    {

        planks.Add(plankCoor);

       //Render  bridgetiles.renderTiles(bridgesTilemap, planks);

    }
    
    public void removeLastPlank()
    {
        if (hasEnd) hasEnd = false;

        Vector2Int removedCoor = lastPlank();
        planks.RemoveAt(planks.Count - 1);

        Vector3Int cellCoor = bridgesTilemap.WorldToCell(removedCoor);
        bridgesTilemap.SetTile(cellCoor, null);
        //bridgesTilemap.RefreshTile(cellCoor);
        bridgetiles.renderTiles(bridgesTilemap, planks);
    }
    public void reverseBridge()
    {
        planks.Reverse();
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
        if (Contains(playerPos) && Contains(targetPos))
        {
            //If there's only one plank left.
            if (planks.Count == 1)
                return playerPos.Equals(lastPlank());
            //Else if the player is moving from last Plank to plank before last.
            return (playerPos.Equals(lastPlank()) && targetPos.Equals(beforeLastPlank()));
        }
        else
            return false;
    }

    public bool Contains(Vector3Int plankPos)
    {
        return planks.Contains(plankPos);
    }
    



}
