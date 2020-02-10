namespace Cawotte.Toolbox.Pathfinding 
{
    using UnityEngine;
    using System;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Class used to save and use the data read from tilemaps. 
    /// </summary>
    [Serializable]
    
    public class Map
    {

        private Grid grid;

        //World Bounds of the map
        private Bounds bounds;
        
        //Cell Bounds of the map (Grid ref)
        private BoundsInt cellBounds;

        private Tilemap[] obstacleTilemaps;

        [SerializeField]
        //Contains all the tiles.
        private Serializable2DArray<TileNode> mapGrid;


        #region Properties
        public Grid Grid
        {
            get
            {
                return grid;
            }
        }

        public TileNode this[int i, int j]
        {
            get
            {
                return mapGrid[i, j];
            }
            set
            {
                mapGrid[i, j] = value;
            }
        }

        public TileNode this[Vector2Int gridPos]
        {
            get
            {
                return this[gridPos.x, gridPos.y];
            }
            set
            {
                this[gridPos.x, gridPos.y] = value;
            }
        }

        public int Width
        {
            get
            {
                return mapGrid.Width;
            }
        }

        public int Height
        {
            get
            {
                return mapGrid.Height;
            }

        }

        public Bounds Bounds { get => bounds; }
        public BoundsInt CellBounds { get => cellBounds; }

    #endregion

        public Map(Grid grid, Tilemap obstacleTilemap)
        {
            this.grid = grid;
            this.obstacleTilemaps = new[] {obstacleTilemap};

            LoadMap();
        }
        
        public Map(Grid grid, Tilemap[] obstacleTilemaps)
        {
            this.grid = grid;
            this.obstacleTilemaps = obstacleTilemaps;

            LoadMap();
        }


        #region Boolean Methods
        /// <summary>
        /// Return true if the given grid coordinate is within cell bounds.
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public bool IsInBounds(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.y >= 0
                && gridPos.x < Width && gridPos.y < Height;
        }

        public bool IsInBounds(Vector3 worldPos)
        {
            return IsInBounds(GetTileIndexAt(worldPos));
        }


        #endregion
        #region Tiles getter
        /// <summary>
        /// Return the tile at the given cell position
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        public TileNode GetTileAt(Vector3Int cellPos)
        {
            //Else calculate its grid coordinate from the cell ones.
            //Vector3Int gridPos = cellPos - mapGrid[0, 0].CellPos;
            Vector2Int indexPos = GetTileIndexAt(cellPos);
            
            if (!IsInBounds(indexPos))
            {
                return null;
            }
            return this[indexPos];
        }
        
        /// <summary>
        /// Return the tile at the given world position.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public TileNode GetTileAt(Vector3 worldPos)
        {
            return GetTileAt(grid.WorldToCell(worldPos));
        }

        /// <summary>
        /// Return the tile (i,j) index (map[i,j]) at the given cell pos.
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        public Vector2Int GetTileIndexAt(Vector3Int cellPos)
        {
            Vector3Int index3 = cellPos - cellBounds.min;
            return new Vector2Int(index3.x, index3.y);
        }

        /// <summary>
        /// Return the tile (i,j) index (tile = map[i,j]) at the given world pos.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector2Int GetTileIndexAt(Vector3 worldPos)
        {
            return GetTileIndexAt(grid.WorldToCell(worldPos));
        }
        #endregion

        public Vector3 GetRandomEmptyPosition()
        {
            TileNode tile;
            do
            {
                tile = this[
                    UnityEngine.Random.Range(0, Width),
                    UnityEngine.Random.Range(0, Height)];
            } while (tile.IsObstacle || tile.CenterWorld.magnitude > 10f);

            return tile.CenterWorld;
        }
        

        /// <summary>
        /// Return the tile at the given CellPos by reading tilemaps.
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        private TileNode GetTileFromTilemap(Vector3Int cellPos)
        {
            bool hasTile = false;

            foreach (Tilemap tilemap in obstacleTilemaps)
            {
                if (tilemap.HasTile(cellPos))
                {
                    hasTile = true;
                    break;
                }
            }
            
            return new TileNode(
                cellPos,
                grid.GetCellCenterWorld(cellPos),
                hasTile);
            
        }

        /// <summary>
        /// Initialize the Map by reading tilemaps.
        /// </summary>
        private void LoadMap()
        {
            //Get Cell Bounds and world bounds
            this.bounds = new Bounds();
            this.cellBounds = new BoundsInt();

            foreach (Tilemap tilemap in obstacleTilemaps)
            {
                bounds.Encapsulate(tilemap.localBounds);
            }
            
            //Get full cell bounds
            cellBounds.SetMinMax(grid.WorldToCell(bounds.min), grid.WorldToCell(bounds.max));
            

            //We initialize a grid
            this.mapGrid = new Serializable2DArray<TileNode>(cellBounds.size.x, cellBounds.size.y);

            Vector3Int cellPos = Vector3Int.zero;
            int xIndex;
            int yIndex = 0;
            for (int j = cellBounds.min.y; j < cellBounds.max.y; j++)
            {
                xIndex = 0;
                for (int i = cellBounds.min.x; i < cellBounds.max.x; i++)
                {
                    cellPos.x = i;
                    cellPos.y = j;
                    mapGrid[xIndex, yIndex] = GetTileFromTilemap(cellPos);
                    xIndex++;
                }
                yIndex++;
            }

        }


    }
}
