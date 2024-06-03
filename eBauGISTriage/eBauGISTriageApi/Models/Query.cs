using Newtonsoft.Json;
using System;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents a query used in the GIS system.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Gets the ID of the query.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int Id { get; }

        /// <summary>
        /// Gets the name of the query.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Name { get; }

        /// <summary>
        /// Gets or sets the PostGIS query. It represents the SQL statement.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string QueryPostGIS { get; set; }

        /// <summary>
        /// Gets or sets the query fields.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string QueryFelder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="id">The ID of the query.</param>
        /// <param name="name">The name of the query.</param>
        /// <param name="queryPostGIS">The PostGIS query representing the SQL statement.</param>
        /// <param name="queryFelder">The query fields.</param>
        public Query(int id, string name, string queryPostGIS, string queryFelder)
        {
            this.Id = id;
            this.Name = name;
            this.QueryPostGIS = queryPostGIS;
            this.QueryFelder = queryFelder;
        }

        public override bool Equals(object? obj)
        {
            return obj is Query query &&
                   Id == query.Id &&
                   Name == query.Name &&
                   QueryPostGIS == query.QueryPostGIS &&
                   QueryFelder == query.QueryFelder;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, QueryPostGIS, QueryFelder);
        }
    }
}
