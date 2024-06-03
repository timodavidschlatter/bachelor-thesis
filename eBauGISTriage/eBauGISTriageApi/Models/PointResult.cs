using System;
using System.Collections.Generic;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents a GIS result associated with a point.
    /// </summary>
    public class PointResult : GisResult
    {
        /// <summary>
        /// Gets or sets the list of queries associated with the GIS result.
        /// </summary>
        public static List<Query> Queries { get; set; }
        private Point? _point;

        /// <summary>
        /// Initializes static members of the <see cref="PointResult"/> class.
        /// </summary>
        static PointResult()
        {
            Queries = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointResult"/> class.
        /// </summary>
        public PointResult() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointResult"/> class with the specified point.
        /// </summary>
        /// <param name="point">The point associated with the GIS result.</param>
        public PointResult(Point point) : base()
        {
            this._point = point;
        }

        /// <summary>
        /// Gets the geometry representation of the GIS result.
        /// </summary>
        /// <returns>The geometry representation of the GIS result. 
        /// If _point is null, it returns null</returns>
        public override string? GetGeometry()
        {
            if (_point is null)
            {
                return null;
            }
            return _point.CoordinateX + " " + _point.CoordinateY;
        }

        public override List<Query> GetQueries()
        {
            return Queries;
        }

        /// <summary>
        /// Checks if a query should belong to the point result. 
        /// </summary>
        /// <param name="query">The query that should be checked.</param>
        /// <returns>true: Query is executed with a point, false: Query is not executed with a point.</returns>
        public static bool AddQueryToResult(Query query)
        {
            return query.Id < 100;
        }

        public override bool Equals(object? obj)
        {
            return obj is PointResult result &&
                   base.Equals(obj) &&
                   EqualityComparer<Point>.Default.Equals(_point, result._point);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _point);
        }
    }
}
