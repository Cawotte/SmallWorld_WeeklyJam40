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


    public int Size
    {
        get
        {
            return planks.Count;
        }
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

    public void RemovePlank(Vector3Int plankCell)
    {
        planks.Remove(plankCell);
    }
    
    public bool RemoveLastPlank()
    {
        if (hasEnd) hasEnd = false;

        Vector3Int plankCell = LastPlank;
        //Vector3Int plankCell = bridgesTilemap.WorldToCell(removedCoor);

        planks.RemoveAt(planks.Count - 1);

        //bridgesTilemap.SetTile(plankCell, null);
        //bridgesTilemap.RefreshTile(cellCoor);
        //bridgetiles.renderTiles(bridgesTilemap, planks);

        //True if empty
        return (planks.Count == 0);
    }


    public bool Contains(Vector3Int plankPos)
    {
        return planks.Contains(plankPos);
    }
    



}
