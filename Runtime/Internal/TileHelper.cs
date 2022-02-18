using UnityEngine;
using UnityEngine.Tilemaps;

namespace Simple2D_AStar.Internal
{
    internal static class TileHelper
    {
        #region Public Functions
        /// <summary>
        /// Calculates the world offset from the 
        /// tile position to the center of the tile.
        /// </summary>
        /// <param name="tilemap">Tilemap to calculate the offset for.</param>
        /// <returns>World offset.</returns>
        public static Vector3 GetTileCenterOffset(Tilemap tilemap)
        {
            Vector3 cellSize = tilemap.transform.TransformVector(tilemap.cellSize);

            Vector3 visualOffset = tilemap.cellLayout switch {

                GridLayout.CellLayout.Rectangle => 
                new Vector3(cellSize.x, cellSize.y, 0) * .5f,

                GridLayout.CellLayout.Isometric => 
                new Vector3(0, cellSize.y * .5f, 0),

                GridLayout.CellLayout.IsometricZAsY => 
                new Vector3(0, cellSize.y * .5f, 0),

                _ => Vector3.zero,
            };

            return SolveCellSwizzle(visualOffset, tilemap.layoutGrid.cellSwizzle);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Converts global x,y,z to grid swizzle x,y,z.
        /// </summary>
        /// <param name="position">Position to convert.</param>
        /// <param name="swizzle">Grid swizzle.</param>
        /// <returns></returns>
        private static Vector3 SolveCellSwizzle(Vector3 position, GridLayout.CellSwizzle swizzle)
        {
            return swizzle switch {

                GridLayout.CellSwizzle.XZY => new Vector3(position.x, position.z, position.y),
                GridLayout.CellSwizzle.ZXY => new Vector3(position.z, position.x, position.y),
                GridLayout.CellSwizzle.ZYX => new Vector3(position.x, position.y, position.x),
                GridLayout.CellSwizzle.YZX => new Vector3(position.y, position.z, position.x),
                GridLayout.CellSwizzle.YXZ => new Vector3(position.y, position.x, position.z),

                _ => position,
            };
        }
        #endregion
    }
}
