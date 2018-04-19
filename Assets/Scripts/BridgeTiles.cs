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

    public void renderTiles(Tilemap bridgesTilemap, List<Vector2> plankCoords) 
    {
        Tile bridgeTile;
        Vector3Int cellCoor;
        Vector2 direction;
        Vector2 direction2;
        

        if ( plankCoords.Count == 1 )
        {
            bridgeTile = ScriptableObject.CreateInstance<Tile>();
            bridgeTile.sprite = defaultSprite;

            cellCoor = bridgesTilemap.WorldToCell(plankCoords[0]);
            bridgesTilemap.SetTile(cellCoor, bridgeTile);
        }
        else
        {
            for ( int i = 0; i < plankCoords.Count; i++)
            {

                bridgeTile = ScriptableObject.CreateInstance<Tile>();

                //First plank rendering
                if (i == 0)
                {
                    direction = plankCoords[1] - plankCoords[0];
                    //Debug.Log("i = 0; Dir = " + direction);
                    bridgeTile.sprite = firstTile(direction);
                }
                //Last plank rendering
                else if (i == plankCoords.Count - 1)
                {
                    direction = plankCoords[plankCoords.Count - 1] - plankCoords[plankCoords.Count - 2];
                    //Debug.Log("i = last; Dir = " + direction);
                    bridgeTile.sprite = endTile(direction);
                }
                //Mid planks renderings.
                else
                {
                    direction = plankCoords[i] - plankCoords[i - 1];
                    direction2 = plankCoords[i] - plankCoords[i + 1];
                    bridgeTile.sprite = midTile(direction, direction2);
                }

                cellCoor = bridgesTilemap.WorldToCell(plankCoords[i]);
                bridgesTilemap.SetTile(cellCoor, bridgeTile);
            }
        }

        bridgesTilemap.RefreshAllTiles();
            
    }

    //We compare the direction of the tile before and after the current tile to choose which tile to assign.
    private Sprite midTile(Vector2 dir1, Vector2 dir2)
    {
        //Debug.Log("dir1 = " + dir1 + ", dir2 = " + dir2);
        //We use ( a || b) because the order doesn't matter. It will never be both the same anyway.
        if ( dir1 == Vector2.up || dir2 == Vector2.up )
        {
            if (dir1 == Vector2.down || dir2 == Vector2.down)
                return vertical;
            else if (dir1 == Vector2.left || dir2 == Vector2.left)
                return corner1;
            else if (dir1 == Vector2.right || dir2 == Vector2.right)
                return corner2;
        }
        else if (dir1 == Vector2.left || dir2 == Vector2.left)
        {
            if (dir1 == Vector2.right || dir2 == Vector2.right)
                return horizontal;
            else if (dir1 == Vector2.down || dir2 == Vector2.down)
                return corner4;
        }
        else if (dir1 == Vector2.right || dir2 == Vector2.right)
        {
            if (dir1 == Vector2.down || dir2 == Vector2.down)
                return corner3;
        }

        return defaultSprite;
    }
    
    private Sprite endTile(Vector2 direction)
    {
        if (direction == Vector2.up)
            return topEnd;
        else if (direction == Vector2.down)
            return bottomEnd;
        else if (direction == Vector2.left)
            return leftEnd;
        else if (direction == Vector2.right)
            return rightEnd;
        else
            return defaultSprite;
    }

    private Sprite firstTile(Vector2 direction)
    {
        if (direction == Vector2.up)
            return bottomEnd;
        else if (direction == Vector2.down)
            return topEnd;
        else if (direction == Vector2.left)
            return rightEnd;
        else if (direction == Vector2.right)
            return leftEnd;
        else
            return defaultSprite;
    }



}
