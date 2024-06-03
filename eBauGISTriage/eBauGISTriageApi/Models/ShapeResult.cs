using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents a shape result in the GIS system.
    /// </summary>
    public class ShapeResult : GisResult
    {
        /// <summary>
        /// Gets or sets the list of queries associated with shape results.
        /// </summary>
        public static List<Query> Queries { get; set; }

        private readonly string? _shape;

        /// <summary>
        /// Initializes static members of the <see cref="ShapeResult"/> class.
        /// </summary>
        static ShapeResult()
        {
            Queries = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeResult"/> class.
        /// </summary>
        public ShapeResult() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeResult"/> class with a shape.
        /// </summary>
        /// <param name="shape">The shape string.</param>
        public ShapeResult(string shape) : base()
        {
            this._shape = shape;
        }

        public override string? GetGeometry()
        {
            return this._shape;
        }

        public override List<Query> GetQueries()
        {
            return Queries;
        }

        /// <summary>
        /// Checks if a query should belong to the shape result. 
        /// </summary>
        /// <param name="query">The query that should be checked.</param>
        /// <returns>true: Shape is executed with a shape string, false: Shape is not executed with a shape string.</returns>
        public static bool AddQueryToResult(Query query)
        {
            return query.Id >= 200;
        }

        public override bool Equals(object? obj)
        {
            return obj is ShapeResult result &&
                   base.Equals(obj) &&
                   EqualityComparer<string>.Default.Equals(_shape, result._shape);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _shape);
        }
    }
}
