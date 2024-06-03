using System;
using System.Data;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// Represents the interface for interacting with the Gdwh database.
    /// </summary>
    public interface IGdwhRepository : IDisposable
    {
        /// <summary>
        /// Reads data from a specific query in the Gdwh database.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <returns>A DataTable containing the result of the query.</returns>
        DataTable ReadDataFromSpecificQuery(string query);
    }
}
