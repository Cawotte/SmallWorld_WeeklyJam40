using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BridgeTiles")]
public class BridgeTiles : ScriptableObject {

    public Sprite defaultSprite;
    public Sprite horizontal;
    public Sprite vertical;
    public Sprite bottomEnd;
    public Sprite topEnd;
    public Sprite leftEnd;
    public Sprite rightEnd;
    public Sprite corner1;
    public Sprite corner2;
    public Sprite corner3;
    public Sprite corner4;

    public void RenderBridge(Tilemap bridgesTilemap, Bridge bridge) 
    {
        Tile bridgeTile;
        Vector3Int cellCoor;
        Vector3Int direction;
        Vector3Int direction2;

        List<Vector3Int> planks = bridge.Planks;
        

        if ( planks.Count == 1 )
        {
            bridgeTile = ScriptableObject.CreateInstance<Tile>();
            bridgeTile.sprite = defaultSprite;

            cellCoor = bridgesTilemap.WorldToCell(planks[0]);
            bridgesTilemap.SetTile(cellCoor, bridgeTile);
        }
        else
        {
            for ( int i = 0; i < planks.Count; i++)
            {

                bridgeTile = ScriptableObject.CreateInstance<Tile>();

                //First plank rendering
                if (i == 0)
                {
                    direction = planks[1] - planks[0];
                    //Debug.Log("i = 0; Dir = " + direction);
                    bridgeTile.sprite = FirstTile(direction);
                }
                //Last plank rendering
                else if (i == planks.Count - 1)
                {
                    direction = planks[planks.Count - 1] - planks[planks.Count - 2];
                    //Debug.Log("i = last; Dir = " + direction);
                    bridgeTile.sprite = EndTile(direction);
                }
                //Mid planks renderings.
                else
                {
                    direction = planks[i] - planks[i - 1];
                    direction2 = planks[i] - planks[i + 1];
                    bridgeTile.sprite = MidTile(direction, direction2);
                }

                cellCoor = bridgesTilemap.WorldToCell(planks[i]);
                bridgesTilemap.SetTile(cellCoor, bridgeTile);
            }
        }

        bridgesTilemap.RefreshAllTiles();
            
    }

    //We compare the direction of the tile before and after the current tile to choose which tile to assign.
    private Sprite MidTile(Vector3Int dir1, Vector3Int dir2)
    {
        //Debug.Log("dir1 = " + dir1 + ", dir2 = " + dir2);
        //We use ( a || b) because the order doesn't matter. It will never be both the same anyway.
        if ( dir1 == Vector3Int.up || dir2 == Vector3Int.up )
        {
            if (dir1 == Vector3Int.down || dir2 == Vector3Int.down)
                return vertical;
            else if (dir1 == Vector3Int.left || dir2 == Vector3Int.left)
                return corner1;
            else if (dir1 == Vector3Int.right || dir2 == Vector3Int.right)
                return corner2;
        }
        else if (dir1 == Vector3Int.left || dir2 == Vector3Int.left)
        {
            if (dir1 == Vector3Int.right || dir2 == Vector3Int.right)
                return horizontal;
            else if (dir1 == Vector3Int.down || dir2 == Vector3Int.down)
                return corner4;
        }
        else if (dir1 == Vector3Int.right || dir2 == Vector3Int.right)
        {
            if (dir1 == Vector3Int.down || dir2 == Vector3Int.down)
                return corner3;
        }

        return defaultSprite;
    }
    
    private Sprite EndTile(Vector3Int direction)
    {
        if (direction == Vector3Int.up)
            return topEnd;
        else if (direction == Vector3Int.down)
            return bottomEnd;
        else if (direction == Vector3Int.left)
            return leftEnd;
        else if (direction == Vector3Int.right)
            return rightEnd;
        else
            return defaultSprite;
    }

    private Sprite FirstTile(Vector3Int direction)
    {
        if (direction == Vector3Int.up)
            return bottomEnd;
        else if (direction == Vector3Int.down)
            return topEnd;
        else if (direction == Vector3Int.left)
            return rightEnd;
        else if (direction == Vector3Int.right)
            return leftEnd;
        else
            return defaultSprite;
    }



}
