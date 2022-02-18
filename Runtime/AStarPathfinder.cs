using Simple2D_AStar.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Simple2D_AStar
{
    public class AStarPathfinder : MonoBehaviour
    {
        #region Data
        public AStarGraph graph;
        #endregion

        #region Public Functions
        /// <summary>
        /// Checks if a position is in the bounds of the graph.
        /// </summary>
        /// <param name="position">Position to check.</param>
        public bool PositionInBounds(Vector3 position) => graph.PositionInBounds(position);
        /// <summary>
        /// Checks if a position is on a graph tile.
        /// </summary>
        /// <param name="position">Position to check.</param>
        public bool PositionInGraph(Vector3 position) => graph.PositionOnGraph(position);

        /// <summary>
        /// Finds a path from the start position to the end position if one exists.
        /// </summary>
        /// <param name="startPosition">Position to start from.</param>
        /// <param name="endPosition">Position to end at.</param>
        /// <returns>Vector3 path to go from start position to end position in world space.</returns>
        public Vector3[] FindPath(Vector3 startPosition, Vector3 endPosition)
        {
            // Finding the nearest node to ensure there are target nodes.
            GraphNode startNode = graph.FindNearestNode(startPosition);
            GraphNode endNode = graph.FindNearestNode(endPosition);

            // The open list is the collection of every node left to search through.
            Heap<GraphNode> openList = new Heap<GraphNode>(graph.GraphCount);
            // The close list is every node that has been searched along with the node it was connect to in the search.
            Dictionary<GraphNode, GraphNode> closeList = new Dictionary<GraphNode, GraphNode>();

            // Adding the start node to the open list to start searching from there.
            openList.Add(startNode);

            while (openList.Count > 0) {

                GraphNode currentNode = openList.RemoveFirst();

                // We have found the path and rebuilding using the close list.
                if (currentNode.tilePosition == endNode.tilePosition) {
                    return RebuildPath(closeList, endNode, startNode);
                }

                // Finding neighbours we have not searched yet.
                foreach (GraphNode neighbour in currentNode.builtNeighbours)
                    if (!closeList.ContainsKey(neighbour)) {

                        // Calculating hscore and gscore to find the best node to search next.
                        neighbour.HScore = currentNode.HScore + CalculateDistance(currentNode.tilePosition, neighbour.tilePosition);
                        neighbour.GScore = CalculateDistance(neighbour.tilePosition, endNode.tilePosition);

                        // Adding neighbour to the search list.
                        openList.Add(neighbour);
                        // connecting the neighbour node to it's 'parent' that helps build the final path.
                        closeList.Add(neighbour, currentNode);
                    }
            }

            // Path was not found.
            return new Vector3[0];
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Rebuilds the path from the end node back to the start node.
        /// </summary>
        /// <param name="closeList">Close list to rebuild the path from.</param>
        /// <param name="endNode">The end node in the path.</param>
        /// <param name="startNode">The start node in the path.</param>
        /// <returns>Vector3 path to get from start node position to end node position.</returns>
        private Vector3[] RebuildPath(Dictionary<GraphNode, GraphNode> closeList, GraphNode endNode, GraphNode startNode)
        {
            // Used to track where we currently are in the close list.
            GraphNode currentNode = endNode;

            // The rebuilt path from the close list.
            List<Vector3> finalPath = new List<Vector3>() {
                graph.NodeToWorld(endNode),
            };

            while (currentNode.tilePosition != startNode.tilePosition) {

                currentNode = closeList[currentNode];
                finalPath.Add(graph.NodeToWorld(currentNode));
            }

            finalPath.Reverse();
            return finalPath.ToArray();
        }
        /// <summary>
        /// Calculates the GScore for a node.
        /// </summary>
        /// <param name="positionA">Node position.</param>
        /// <param name="positionB">End node position.</param>
        /// <returns>GScore.</returns>
        private float CalculateDistance(Vector2Int positionA, Vector2Int positionB)
        {
            return Vector2Int.Distance(positionA, positionB);
        }
        #endregion
    }
}
