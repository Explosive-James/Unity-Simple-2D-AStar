using System;
using UnityEngine;

namespace Simple2D_AStar.Internal
{
    [Serializable]
    internal class GraphNode : IComparable<GraphNode>
    {
        #region Data
        public Vector2Int tilePosition;

        [NonSerialized]
        public GraphNode[] builtNeighbours;
        public Vector2Int[] savedNeighbours;
        #endregion

        #region Properties
        /// <summary>
        /// The score / distance to the goal.
        /// </summary>
        public float GScore { get; set; }
        /// <summary>
        /// The score / distance to home.
        /// </summary>
        public float HScore { get; set; }

        /// <summary>
        /// Estimated score / distance to the final path.
        /// </summary>
        public float FScore => GScore + HScore;
        #endregion

        #region Constructor
        public GraphNode(Vector2Int tilePosition)
        {
            this.tilePosition = tilePosition;

            savedNeighbours = new Vector2Int[0];
            builtNeighbours = new GraphNode[0];
        }
        #endregion

        #region Public Functions
        public int CompareTo(GraphNode other)
        {
            return FScore.CompareTo(other.FScore);
        }
        #endregion
    }
}
