using eBauGISTriageApi.Models;
using System.Collections.Generic;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// Represents the interface for accessing and retrieving queries.
    /// </summary>
    public interface IQueriesRepository
    {
        /// <summary>
        /// Reads and returns all existing queries.
        /// </summary>
        /// <returns>A dictionary with the query id as key and the query object as value.</returns>
        Dictionary<int, Query> GetAllQueries();
    }
}
