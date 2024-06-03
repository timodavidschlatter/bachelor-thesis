using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents the results of a query in the GIS system.
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Gets or sets the ID of the query.
        /// </summary>
        public int QueryId { get; set; }

        /// <summary>
        /// Gets or sets the name of the query.
        /// </summary>
        public string QueryName { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of query results.
        /// </summary>
        public Dictionary<string, List<string>> Results { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult"/> class.
        /// </summary>
        /// <param name="queryId">The ID of the query.</param>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="results">The dictionary of query results.</param>
        public QueryResult(int queryId, string queryName, Dictionary<string, List<string>> results)
        {
            this.QueryId = queryId;
            this.QueryName = queryName;
            this.Results = results;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not QueryResult)
            {
                return false;
            }

            QueryResult result = (QueryResult)obj;
            string dict1string = string.Join(",", this.Results.OrderBy(kv => kv.Key).Select(kv => kv.Key + ":" + string.Join("|", kv.Value.OrderBy(v => v))));
            string dict2string = string.Join(",", result.Results.OrderBy(kv => kv.Key).Select(kv => kv.Key + ":" + string.Join("|", kv.Value.OrderBy(v => v))));

            return this.QueryId == result.QueryId && 
                   this.QueryName == result.QueryName &&
                   dict1string.Equals(dict2string);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QueryId, QueryName, Results);
        }
    }
}
