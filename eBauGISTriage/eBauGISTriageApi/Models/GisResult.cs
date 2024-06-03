using System;
using System.Collections.Generic;
using System.Linq;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// This model represents a gis result. It is the parent class of
    /// different results, e.g. PointResult, ShapeResult. 
    /// It should not be instantiated directly. 
    /// </summary>
    public class GisResult
    {
        /// <summary>
        /// Gets or sets the list of query results associated with the GIS result.
        /// </summary>
        public List<QueryResult> QueryResults { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GisResult"/> class.
        /// </summary>
        public GisResult()
        {
            QueryResults = new();
        }

        /// <summary>
        /// Returns a string containing the geometry for this type. 
        /// The string is ready out of the box to be inserted in the query.
        /// </summary>
        /// <returns>The string containing the geometry.</returns>
        public virtual string? GetGeometry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of queries for the type of geometry.
        /// </summary>
        /// <returns>The list of queries.</returns>
        public virtual List<Query> GetQueries()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj)
        {
            return obj is GisResult result &&
                Enumerable.SequenceEqual(
                    this.QueryResults.OrderBy(qr => qr.QueryName),
                    result.QueryResults.OrderBy(qr => qr.QueryName));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QueryResults);
        }
    }
}
