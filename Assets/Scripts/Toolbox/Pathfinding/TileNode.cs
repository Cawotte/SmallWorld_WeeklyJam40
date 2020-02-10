namespace Cawotte.Toolbox.Pathfinding 
{
    using UnityEngine;
    using System;

    /// <summary>
    /// Class that hold a tile data, read from a tilemap.
    /// </summary>
    [Serializable]
    public class TileNode
    {
        [SerializeField][ReadOnly]
        private Vector3Int cellPos; //Pos of the cell in the Grid.

        [SerializeField][ReadOnly]
        private Vector3 centerWorld; //Pos of the cell in World.

        [SerializeField]
        [ReadOnly]
        private bool isObstacle;

        #region Properties
        public Vector3Int CellPos { get => cellPos; }
        public Vector3 CenterWorld { get => centerWorld; }
        public bool IsObstacle { get => isObstacle;  }
        #endregion

        public TileNode(Vector3Int cellPos, Vector3 centerWorld, bool isObstacle)
        {
            this.cellPos = cellPos;
            this.centerWorld = centerWorld;
            this.isObstacle = isObstacle;
        }

        /// <summary>
        /// Use the bottom center of the tile + a random X offset to get a spawn point in that tile.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomSpawnPoint()
        {
            return centerWorld +
                new Vector3(
                    UnityEngine.Random.Range(-0.4f, 0.4f),
                    -0.5f,
                    0f);
        }
    }
}
