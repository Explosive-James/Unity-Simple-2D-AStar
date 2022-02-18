
namespace Simple2D_AStar
{
    public enum DirectionType : byte
    {
        /// <summary>
        /// Moves to any neighbouring tiles.
        /// </summary>
        Omnidirectional,
        /// <summary>
        /// Only North, East, South West.
        /// </summary>
        OrthogonalOnly,
        /// <summary>
        /// Only NorthEast, SouthEast, NorthWest, SouthWest.
        /// </summary>
        DiagonalOnly,
        /// <summary>
        /// Only moves orthogonally when neighbouring diagonal tiles are traversable.
        /// </summary>
        SmartOrthogonal,
        /// <summary>
        /// Only moves diagonal when neighbouring orthogonal tiles are traversable.
        /// </summary>
        SmartDiagonal,
    }
}
