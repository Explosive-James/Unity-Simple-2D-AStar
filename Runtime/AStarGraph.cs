using Simple2D_AStar.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Simple2D_AStar
{
    public sealed class AStarGraph : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Data
        /// <summary>
        /// Tilemap that controls where nodes are placed.
        /// </summary>
        public Tilemap walkableTiles;
        /// <summary>
        /// Controls how nodes can connect to each other.
        /// </summary>
        public DirectionType directionType;

        /// <summary>
        /// Should the graph be built / updated when the project is built.
        /// </summary>
        public bool generateOnBuild = true;

        private Dictionary<Vector2Int, GraphNode> builtGraph = new Dictionary<Vector2Int, GraphNode>();
        [SerializeField] private List<GraphNode> savedGraph = new List<GraphNode>();
        #endregion

        #region Properties
        /// <summary>
        /// How many nodes exist in the graph.
        /// </summary>
        public int GraphCount => builtGraph.Count;
        #endregion

        #region Unity Functions
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (GraphNode node in builtGraph.Values) {

                Vector3 worldPosition = NodeToWorld(node);
                Gizmos.DrawCube(worldPosition, Vector3.one * .1f);

                foreach (GraphNode neighbour in node.builtNeighbours) {

                    Vector3 neighbourPosition = NodeToWorld(neighbour);
                    Gizmos.DrawLine(worldPosition, Vector3.Lerp(worldPosition, neighbourPosition, .5f));
                }
            }
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            savedGraph = new List<GraphNode>();

            foreach (GraphNode node in builtGraph.Values)
                savedGraph.Add(node);

            foreach (GraphNode node in savedGraph) {

                node.savedNeighbours = new Vector2Int[node.builtNeighbours.Length];

                for (int i = 0; i < node.savedNeighbours.Length; i++)
                    node.savedNeighbours[i] = node.builtNeighbours[i].tilePosition;
            }
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            builtGraph = new Dictionary<Vector2Int, GraphNode>();

            foreach (GraphNode node in savedGraph)
                builtGraph.Add(node.tilePosition, node);

            foreach (GraphNode node in savedGraph) {

                node.builtNeighbours = new GraphNode[node.savedNeighbours.Length];

                for (int i = 0; i < node.builtNeighbours.Length; i++)
                    node.builtNeighbours[i] = builtGraph[node.savedNeighbours[i]];
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Generates / updates the navigation graph used for pathfinding.
        /// </summary>
        public void UpdateGraph()
        {
            builtGraph = new Dictionary<Vector2Int, GraphNode>();
            Vector3Int initialPosition = walkableTiles.cellBounds.position;

            for (int x = 0; x < walkableTiles.size.x; x++)
                for (int y = 0; y < walkableTiles.size.y; y++) {

                    Vector3Int tilePosition = initialPosition + new Vector3Int(x, y, 0);

                    if (walkableTiles.HasTile(tilePosition)) {

                        GraphNode node = new GraphNode((Vector2Int)tilePosition);
                        builtGraph.Add(node.tilePosition, node);
                    }
                }

            foreach (GraphNode node in builtGraph.Values) {

                node.builtNeighbours = NeighbourHelper.GetNeighbours(
                    builtGraph, node, walkableTiles.cellLayout, directionType);
            }
        }

        /// <summary>
        /// Checks if a position is in the bounds of the graph.
        /// </summary>
        /// <param name="position">Position to check.</param>
        public bool PositionInBounds(Vector3 position)
        {
            Vector3Int tilePosition = walkableTiles.WorldToCell(position) -
                walkableTiles.cellBounds.position;

            return tilePosition.x >= 0 && tilePosition.x < walkableTiles.size.x &&
                tilePosition.y >= 0 && tilePosition.y < walkableTiles.size.y;
        }
        /// <summary>
        /// Checks if a position is on a graph tile.
        /// </summary>
        /// <param name="position">Position to check.</param>
        public bool PositionOnGraph(Vector3 position)
        {
            Vector3Int tilePosition = walkableTiles.WorldToCell(position);
            return builtGraph.ContainsKey((Vector2Int)tilePosition);
        }
        #endregion

        #region Internal Functions
        /// <summary>
        /// Converts a graph node position to world space.
        /// </summary>
        /// <param name="node">Node to convert the position of.</param>
        /// <returns>World position of the graph node.</returns>
        internal Vector3 NodeToWorld(GraphNode node)
        {
            Vector3 worldOffset = TileHelper.GetTileCenterOffset(walkableTiles);
            return walkableTiles.CellToWorld((Vector3Int)node.tilePosition) + worldOffset;
        }

        /// <summary>
        /// Finds the nearest graph node at a given position.
        /// </summary>
        /// <param name="position">Position in world space.</param>
        /// <returns>Nearest node.</returns>
        internal GraphNode FindNearestNode(Vector3 position)
        {
            // Converting world position to tile position.
            Vector2Int tilePosition = (Vector2Int)walkableTiles.WorldToCell(position);

            // Checking if the position is already on a tile.
            if (builtGraph.ContainsKey(tilePosition)) {
                return builtGraph[tilePosition];
            }

            float bestDistance = Mathf.Infinity;
            Vector2Int bestTile = Vector2Int.zero;

            foreach (Vector2Int tile in builtGraph.Keys) {

                float distance = Vector2Int.Distance(tile, tilePosition);

                if (distance < bestDistance) {

                    bestDistance = distance;
                    bestTile = tile;
                }
            }

            return builtGraph[bestTile];
        }
        #endregion
    }
}
