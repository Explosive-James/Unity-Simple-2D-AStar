using System.Collections.Generic;
using UnityEngine;
using System;

namespace Simple2D_AStar.Internal
{
    internal static class NeighbourHelper
    {
        #region Data
        private static readonly Vector2Int[] orthogonals = new Vector2Int[] {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
        };
        private static readonly Vector2Int[] diagonals = new Vector2Int[] {
            new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, 1), new Vector2Int(1, -1),
        };
        #endregion

        #region Public Functions
        public static GraphNode[] GetNeighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node,
            GridLayout.CellLayout layout, DirectionType direction)
        {
            // Handling hexagon as a special case.
            if (layout == GridLayout.CellLayout.Hexagon)
                return ValidateHexagonNeighbours(graph, node);

            List<GraphNode> neighbours = new List<GraphNode>();

            switch (direction) {

                case DirectionType.Omnidirectional:
                    neighbours.AddRange(ValidateOrthogonalNeighbours(graph, node));
                    neighbours.AddRange(ValidateDiagonalNighbours(graph, node));
                    break;

                case DirectionType.OrthogonalOnly:
                    neighbours.AddRange(ValidateOrthogonalNeighbours(graph, node));
                    break;

                case DirectionType.DiagonalOnly:
                    neighbours.AddRange(ValidateDiagonalNighbours(graph, node));
                    break;

                case DirectionType.SmartOrthogonal:
                    neighbours.AddRange(ValidateDiagonalNighbours(graph, node));
                    neighbours.AddRange(ValidateSmartOrthogonalNeighbours(graph, node));
                    break;

                case DirectionType.SmartDiagonal:
                    neighbours.AddRange(ValidateOrthogonalNeighbours(graph, node));
                    neighbours.AddRange(ValidateSmartDiagonalNeighbours(graph, node));
                    break;
            }

            return neighbours.ToArray();
        }
        #endregion

        #region Private Functions
        private static GraphNode[] ValidateHexagonNeighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node)
        {
            List<GraphNode> neighbours = new List<GraphNode>(ValidateOrthogonalNeighbours(graph, node));

            int offset = Math.Abs(node.tilePosition.y) % 2 * 2;

            for (int i = 0; i < 2; i++) {

                Vector2Int position = node.tilePosition + diagonals[i + offset];

                if (graph.ContainsKey(position))
                    neighbours.Add(graph[position]);
            }

            return neighbours.ToArray();
        }
        private static GraphNode[] ValidateOrthogonalNeighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node)
        {
            List<GraphNode> neighbours = new List<GraphNode>();

            foreach (Vector2Int direction in orthogonals) {

                Vector2Int position = node.tilePosition + direction;

                if (graph.ContainsKey(position))
                    neighbours.Add(graph[position]);
            }

            return neighbours.ToArray();
        }
        private static GraphNode[] ValidateDiagonalNighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node)
        {
            List<GraphNode> neighbours = new List<GraphNode>();

            foreach (Vector2Int direction in diagonals) {

                Vector2Int position = node.tilePosition + direction;

                if (graph.ContainsKey(position))
                    neighbours.Add(graph[position]);
            }

            return neighbours.ToArray();
        }
        private static GraphNode[] ValidateSmartOrthogonalNeighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node)
        {
            List<GraphNode> neighbours = new List<GraphNode>();

            foreach (Vector2Int direction in orthogonals) {

                Vector2Int[] diagonals = GetDiagonalsFromOrthogonal(direction);
                Vector2Int tilePosition = node.tilePosition + direction;

                for (int i = 0; i < diagonals.Length; i++)
                    diagonals[i] += node.tilePosition;

                if (GraphNodesExist(graph, diagonals) && graph.ContainsKey(tilePosition)) {

                    neighbours.Add(graph[tilePosition]);
                }
            }

            return neighbours.ToArray();
        }
        private static GraphNode[] ValidateSmartDiagonalNeighbours(Dictionary<Vector2Int, GraphNode> graph, GraphNode node)
        {
            List<GraphNode> neighbours = new List<GraphNode>();

            foreach (Vector2Int direction in diagonals) {

                Vector2Int[] tilePositions = new Vector2Int[] {
                    node.tilePosition + new Vector2Int(direction.x, 0),
                    node.tilePosition + new Vector2Int(0, direction.y)
                };
                Vector2Int tilePosition = node.tilePosition + direction;

                if (GraphNodesExist(graph, tilePositions) && graph.ContainsKey(tilePosition)) {

                    neighbours.Add(graph[tilePosition]);
                }
            }

            return neighbours.ToArray();
        }

        private static bool GraphNodesExist(Dictionary<Vector2Int, GraphNode> graph, params Vector2Int[] tilePositions)
        {
            for (int i = 0; i < tilePositions.Length; i++)
                if (!graph.ContainsKey(tilePositions[i]))
                    return false;
            return true;
        }
        private static Vector2Int[] GetDiagonalsFromOrthogonal(Vector2Int orthogonal)
        {
            if (orthogonal.x != 0)
                return new Vector2Int[] {
                    new Vector2Int(orthogonal.x, 1), new Vector2Int(orthogonal.x, -1)
                };
            return new Vector2Int[] {
                new Vector2Int(1, orthogonal.y), new Vector2Int(-1, orthogonal.y),
            };
        }
        #endregion
    }
}
