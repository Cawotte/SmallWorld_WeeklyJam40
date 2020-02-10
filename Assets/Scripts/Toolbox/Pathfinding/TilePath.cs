namespace Cawotte.Toolbox.Pathfinding 
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    /// <summary>
    /// Class that contains the resulting Path of a pathfinding algorithm, as a list of tile.
    /// </summary>
    public class TilePath
    {
        private TileNode[] path;  //Full path
        private TilePath simplifiedPath; //Paths that only contains tiles where a turn need to be made.  
        private Vector3Int[] directionPath; //Path with only the directions to the next tile.

        private int currentTileIndex = 0;

        public TileNode Start
        {
            get => path[0];
        }

        public TileNode Goal
        {
            get => path[path.Length - 1];
        }
        public bool IsEmpty
        {
            get => Size == 0;
        }
        

        public int Size
        {
            get => (path == null) ? 0 : path.Length;
        }

        public TilePath SimplifiedPath { get => simplifiedPath;  }

        public TileNode this[int i]
        {
            get => path[i];
        }

        /// <summary>
        /// Empty path
        /// </summary>
        public TilePath()
        {

        }

        public TilePath(IEnumerable<TileNode> tiles, bool isSimplified = false)
        {
            this.path = tiles.ToArray();

            /*
            LoadDirectionPath();

            if (!isSimplified)
            {
                LoadSimplifiedPath();
             */
        }

        public TileNode Next()
        {
            TileNode next = path[currentTileIndex];
            currentTileIndex++;
            return next;
        }
        
        public bool HasNext()
        {
            return currentTileIndex < Size;
        }

        private void LoadDirectionPath()
        {
            if (IsEmpty) return;

            directionPath = new Vector3Int[Size - 1];

            for (int i = 0; i < Size - 1; i++)
            {
                directionPath[i] = path[i + 1].CellPos - path[i].CellPos;
            }
        }

        private void LoadSimplifiedPath()
        {
            if (IsEmpty) return;

            List<TileNode> simplePath = new List<TileNode>();

            Vector3Int lastDirection = directionPath[0];

            simplePath.Add(Start);

            //We add a node to the path only when the direction change
            for (int i = 1; i < Size; i++)
            {
                if (i == Size - 1)
                {
                    simplePath.Add(path[i]); //goal
                    break;
                }
                if (directionPath[i - 1] != directionPath[i])
                {
                    simplePath.Add(path[i]);
                }
            }

            simplifiedPath = new TilePath(simplePath, true);
        }

    }
}
