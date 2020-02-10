namespace Cawotte.Toolbox.Pathfinding 
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    /// <summary>
    /// Used the A* Algorithm to find a path in a Map.
    /// </summary>
    [System.Serializable]
    public class Pathfinder
    {
        private Map mapGrid;
        
        public bool DiagonalMovements = false;
        public bool FindNearestGoal = true;

        public Pathfinder(Map mapGrid, bool diagonalMovements)
        {
            this.mapGrid = mapGrid;
            this.DiagonalMovements = diagonalMovements;
        }

        //Get a path from start to goal world positions.
        public TilePath GetPath(Vector3 startWorldPos, Vector3 goalWorldPos)
        {

            return new TilePath(GetTilePath(mapGrid.GetTileIndexAt(startWorldPos), mapGrid.GetTileIndexAt(goalWorldPos)));
        }
        

        /// <summary>
        /// Return the cell path from start to goal.
        /// Return an empty path if start = goal.
        /// Return null if there's no existing path.
        /// Use grid positions.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private Stack<TileNode> GetTilePath(Vector2Int startGridPos, Vector2Int endGridPos)
        {
            StarCell start = new StarCell(startGridPos);
            StarCell goal = new StarCell(endGridPos);
            StarCell current;
            

            if (start.Equals(goal))
            {
                return new Stack<TileNode>(); //return empty path.
            }
            if (mapGrid[endGridPos].IsObstacle)
            {
                goal = new StarCell(GetNearestGoal(startGridPos, endGridPos));
            }

            //Keep tracks of the processed locations and unprocessed neighbors. 
            HashSet<StarCell> closedSet = new HashSet<StarCell>();
            HashSet<StarCell> openSet = new HashSet<StarCell>();
            int g = 0;

            openSet.Add(start);

            //FLOOD STOP
            int MAX_ITER = 10000;
            int nbIter = 0;
            while (openSet.Count > 0 && nbIter < MAX_ITER)
            {
                nbIter++;

                //Get the unprocessed location with the lowest FScore. 
                //var lowest = openSet.Min(loc => loc.Fscore);
                //current = openSet.First(loc => loc.Fscore == lowest);
                current = openSet.Aggregate((p1, p2) => p1.FScore < p2.FScore ? p1 : p2);
                
                closedSet.Add(current);
                openSet.Remove(current);

                //If current is the goal cell.
                if (current.Equals(goal))
                {
                    return BuildPathFromTile(current);
                }
                
                //Get valid neighbors
                List<Vector2Int> neighbors = GetWalkableNeighbors(current.GridPosition);
                g++;

                foreach (Vector2Int neighborPos in neighbors)
                {
                    StarCell neighbor = new StarCell(neighborPos);

                    //if this adjacent square is already in the closed list, ignore it
                    if (closedSet.Contains(neighbor))
                        continue;

                    // if it's not in the open list...
                    if (!openSet.Contains(neighbor))
                    {
                        // compute its scores, set the parent
                        neighbor.GScore = g;
                        neighbor.HScore = Vector2Int.Distance(neighbor.GridPosition, goal.GridPosition);
                        neighbor.Parent = current;

                        // and add it to the open list
                        openSet.Add(neighbor);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + neighbor.HScore < neighbor.FScore)
                        {
                            neighbor.GScore = g;
                            neighbor.Parent = current;
                        }
                    }

                }

            }

            Debug.Log("No path or flood ! Flood count : " + nbIter);
            return new Stack<TileNode>();
        }

        #region private Methods

        private Vector2Int GetNearestGoal(Vector2Int start, Vector2Int goal)
        {
            //depth 1
            List<Vector2Int> explored = new List<Vector2Int>();
            Queue<Vector2Int> toExplore = new Queue<Vector2Int>();
            
            toExplore.Enqueue(goal);
            explored.Add(goal);


            while (toExplore.Count > 0)
            {
                Vector2Int currentGoal = toExplore.Dequeue();
                List<Vector2Int> neighbors = GetNeighbors(currentGoal);
                
                //order by and add to explore
                neighbors = neighbors.OrderBy((cell) => Vector2Int.Distance(start, cell)).ToList();
                for (var i = 0; i < neighbors.Count; i++)
                {
                    if (!explored.Contains(neighbors[i]))
                    {
                        toExplore.Enqueue(neighbors[i]);
                    }
                }

                //If next is a valid goal
                if (!explored.Contains(currentGoal))
                {
                    explored.Add(currentGoal);
                    if (IsWalkable(currentGoal))
                    {
                        return currentGoal;
                    }
                }

            }
            
            Debug.Log("No close neighbor found!");
            return goal;

        }
        
        
        private List<Vector2Int> GetWalkableNeighbors(Vector2Int cellPos)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int neighbor;

            Vector2Int[] directions;
            if (DiagonalMovements)
            {
                //8 directions
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,

                    Vector2Int.up + Vector2Int.right,
                    Vector2Int.up + Vector2Int.left,
                    Vector2Int.down + Vector2Int.right,
                    Vector2Int.down + Vector2Int.left,
                };
            }
            else
            {
                //4 directions
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                };
            }


            for (int i = 0; i < directions.Length; i++)
            {
                neighbor = cellPos + directions[i];
                if (mapGrid.IsInBounds(neighbor) && !mapGrid[neighbor].IsObstacle)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int cellPos)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int neighbor;

            Vector2Int[] directions;
            if (DiagonalMovements)
            {
                //8 directions
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,

                    Vector2Int.up + Vector2Int.right,
                    Vector2Int.up + Vector2Int.left,
                    Vector2Int.down + Vector2Int.right,
                    Vector2Int.down + Vector2Int.left,
                };
            }
            else
            {
                //4 directions
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                };
            }


            for (int i = 0; i < directions.Length; i++)
            {
                neighbor = cellPos + directions[i];
                neighbors.Add(neighbor);
            }

            return neighbors;
        }

        private Stack<TileNode> BuildPathFromTile(PathCell goalCell)
        {
            Stack<TileNode> pathTile = new Stack<TileNode>();
            PathCell current = goalCell;

            //Bug if current == null at start
            do
            {
                pathTile.Push(mapGrid[current.GridPosition]);
                current = current.Parent;
            } while (current != null);

            return pathTile;
        }

        private bool IsWalkable(Vector2Int cellPos)
        {
            if (mapGrid[cellPos.x, cellPos.y] == null)
            {
                return false;
            }
            return !mapGrid[cellPos].IsObstacle;
        }

        #endregion


        #region Pathfinding Cell classes

        /// <summary>
        /// Base class that holds the core informations about a tile for the pathfinding algorithms.
        /// </summary>
        private class PathCell
        {
            public Vector2Int GridPosition;
            public PathCell Parent = null;

            public PathCell(Vector2Int gridPos)
            {
                this.GridPosition = gridPos;
            }

            public override bool Equals(object obj)
            {
                //Tiles are diffentiated by their coordinates.
                if (obj is PathCell && obj != null)
                {
                    return GridPosition == ((PathCell)obj).GridPosition;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return GridPosition.GetHashCode();
            }
        }
        /// <summary>
        /// Class that holds the core informations about a tile for the A* algorithm
        /// </summary>
        private class StarCell : PathCell
        {
            public float GScore = 0; //start => this
            public float HScore = 0; //heuristic, this => goal
            public float FScore
            {
                get
                {
                    return GScore + HScore;
                }
            }
            
            public StarCell(Vector2Int gridPos) : base(gridPos)
            {

            }
        }
        #endregion

    }
}
