using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A tool made for the refactor of the whole game.
/// Copy the content of a tilemap into another tilemap. 
/// Can use a list of Tilemap pairs (original/copy) and is usable in Editor through a button
/// </summary>
public class CopyTilemap : MonoBehaviour
{

    [SerializeField]
    private List<TilemapPair> tilemapsToCopy;

    [System.Serializable]
    public struct TilemapPair
    {
        public Tilemap original;
        public Tilemap copy;
    }

    public void ClearAndCopyAllTilemaps()
    {
        foreach (TilemapPair pair in tilemapsToCopy)
        {
            CopyTilemapContent(pair.original, pair.copy);
        }
    }

    private void CopyTilemapContent(Tilemap original, Tilemap toCopy)
    {
        //Empty tilemap to copy in
        toCopy.ClearAllTiles();

        //Get Bounds and Resize Tilemaps
        BoundsInt bounds = original.cellBounds;
        TileBase[] allTiles = original.GetTilesBlock(bounds);

        toCopy.origin = bounds.position;
        toCopy.size = bounds.size;
        toCopy.ResizeBounds();

        toCopy.transform.position = bounds.position;

        //Copy all content
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                toCopy.SetTile(cellPos, tile);
            }
        }

        toCopy.RefreshAllTiles();
    }
}
