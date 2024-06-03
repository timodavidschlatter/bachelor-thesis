using eBauGISTriageApi.Helper.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// Represents a repository for interacting with the Gdwh database.
    /// </summary>
    public class GdwhRepository : IGdwhRepository
    {
        private readonly GdwhCtx _gdwhCtx;
        private readonly ILogger _logger;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GdwhRepository"/> class.
        /// </summary>
        /// <param name="gdwhCtx">The Gdwh database context.</param>
        /// <param name="logger">The logger instance used for logging.</param>
        public GdwhRepository(GdwhCtx gdwhCtx, ILogger logger)
        {
            this._gdwhCtx = gdwhCtx;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a query against the database and returns the result as a DataTable object.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>A DataTable object containing the data from the database.</returns>
        /// <exception cref="CustomException"></exception>
        public DataTable ReadDataFromSpecificQuery(string query)
        {
            this._logger.LogInformation("Connecting to GDWH, executing query and reading data.");

            try
            {
                OpenDbConnection();
                using var dbCommand = this._gdwhCtx.Database.GetDbConnection().CreateCommand();
                NpgsqlCommand cmd = (NpgsqlCommand)dbCommand;
                cmd.AllResultTypesAreUnknown = true;
                cmd.CommandText = query;
                using var reader = cmd.ExecuteReader();

                DataTable result = new();
                result.Load(reader);

                return result;
            } 
            catch (NpgsqlException ex)
            {
                this._logger.LogError("Unexpected database error.");
                this._logger.LogError("Used query: {query}", query);
                this._logger.LogError("Detailed error message: {message}", ex.Message);
                throw new CustomException("NpgsqlException", "Unexpected database error.");
            }
            catch (Exception ex)
            {
                this._logger.LogError("Unexpected error.");
                this._logger.LogError("Used query: " + query);
                this._logger.LogError("Detailed error message: {message}", ex.Message);
                throw new CustomException("Exception", "Unexpected error.");
            }
        }

        /// <summary>
        /// Disposes the repository and releases any resources used.
        /// Source: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the repository and releases any resources used.
        /// Source: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
        /// </summary>
        /// <param name="disposing">Boolean containing information if the context was already disposed or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._gdwhCtx.Dispose();
                }
            }
            this._disposed = true;
        }

        /// <summary>
        /// Opens the database connection if not already in ConnectionState.Open.
        /// Source: GISQueryCheckAPI.
        /// </summary>
        private void OpenDbConnection()
        {
            if (_gdwhCtx.Database.GetDbConnection().State != ConnectionState.Open)
            {
                _gdwhCtx.Database.GetDbConnection().Open(); // will be closed when the context is _disposed by the API controller
            }
        }
    }
}
