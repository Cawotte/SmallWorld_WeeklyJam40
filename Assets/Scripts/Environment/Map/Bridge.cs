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

    public void Reverse()
    {
        planks.Reverse();
    }


    public bool ArePlanksConsecutive(Vector3Int a, Vector3Int b)
    {
        for (int i = 0; i < Size - 1; i++)
        {
            Vector3Int plank = planks[i];
            if (plank.Equals(a) || plank.Equals(b))
            {
                return planks[i + 1].Equals(a) || planks[i + 1].Equals(b);
            }
        }

        return false;
    }
    
    public bool IsAnExtremity(Vector3Int plankCell)
    {
        return plankCell.Equals(planks[0]) || plankCell.Equals(planks[Size - 1]);
    }

    public bool Contains(Vector3Int plankPos)
    {
        return planks.Contains(plankPos);
    }
    



}
