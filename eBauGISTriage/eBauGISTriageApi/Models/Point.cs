namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents a point with X and Y coordinates.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets the X coordinate of the point.
        /// </summary>
        public double CoordinateX { get; }

        /// <summary>
        /// Gets the Y coordinate of the point.
        /// </summary>
        public double CoordinateY { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class.
        /// </summary>
        /// <param name="coordinateX">The X coordinate of the point.</param>
        /// <param name="coordinateY">The Y coordinate of the point.</param>
        public Point(double coordinateX, double coordinateY)
        {
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
        }
    }
}
