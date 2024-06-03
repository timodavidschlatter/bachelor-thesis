using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data.ResponseModel;
using eBauGISTriageApi.Helper.DTO;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBauGISTriageApi.Services
{
    /// <summary>
    /// Service class for executing queries and retrieving results from the GDWH.
    /// </summary>
    public class QueriesService
    {
        private readonly IDbContextFactory<GdwhCtx> _dbContextFactory;
        private readonly ILogger _logger;
        private readonly IQueriesRepository _queriesRepository;

        /// <summary>
        /// Gets or sets the dictionary of queries, where the key is the query id and the value is the query object.
        /// </summary>
        public Dictionary<int, Query>? Queries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueriesService"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The IDbContextFactory for creating GDWH contexts.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="queriesRepository">The repository for accessing query data.</param>
        public QueriesService(
            IDbContextFactory<GdwhCtx> dbContextFactory,
            ILogger<QueriesService> logger,
            IQueriesRepository queriesRepository)
        {
            this._dbContextFactory = dbContextFactory;
            this._logger = logger;
            this._queriesRepository = queriesRepository;
        }

        /// <summary>
        /// Parallel execution of the ExecuteQuery method.
        /// </summary>
        /// <param name="response">The response to add the gis Results to.</param>
        /// <param name="geometryDTO">The DTO object containing the point and the triage geometryDTO.</param>
        /// <returns>The response object with Results.</returns>
        public Response ParallelExecuteQueries(
            Response response,
            GeometryDTO geometryDTO)
        {
            this._logger.LogInformation("Start executing the queries parallel against gdwh.");

            response.GisResults.AddRange(geometryDTO.PointResults);
            response.GisResults.Add(geometryDTO.ShapeResult);

            for (int i = 0; i < response.GisResults.Count; i++)
            {
                GisResult gisResult = response.GisResults[i];

                Parallel.ForEach(
                gisResult.GetQueries(),
                new ParallelOptions { MaxDegreeOfParallelism = 10 },
                () =>
                {
                    // Initialization of taskLocal. Each thread has an own repository
                    // to communicate with the db.
                    GdwhCtx gdwhCtx = this._dbContextFactory.CreateDbContext();
                    IGdwhRepository repository = new GdwhRepository(gdwhCtx, this._logger);
                    List<QueryResult> queryResults = new();

                    return new
                    {
                        GdwhRepository = repository,
                        QueryResults = queryResults,
                    };
                },
                (query, parallelLoopState, longVar, taskLocal) =>
                {
                    QueryResult queryResult = ExecuteQuery(
                        query,
                        gisResult,
                        taskLocal.GdwhRepository);

                    if (queryResult.Results.Count > 0)
                    {
                        taskLocal.QueryResults.Add(queryResult);
                    }

                    return taskLocal;
                },
                (taskLocal) =>
                {
                    taskLocal.GdwhRepository.Dispose();
                    lock (gisResult)
                    {
                        gisResult.QueryResults.AddRange(taskLocal.QueryResults);
                    }
                });
            }

            return response;
        }

        /// <summary>
        /// Executes a query against the GDWH and returns a gis result.
        /// </summary>
        /// <param name="query">The query to execute on the gdwh.</param>
        /// <param name="gisResult">The GisResult object containing the point or the triage string.</param>
        /// <param name="repository">The repository to access database functions.</param>
        /// <returns>The gis result.</returns>
        public QueryResult ExecuteQuery(
            Query query,
            GisResult gisResult,
            IGdwhRepository repository)
        {
            string queryString = query.QueryPostGIS.Replace(
                "xxx yyy", gisResult.GetGeometry());
            queryString = queryString.Replace("-json-", gisResult.GetGeometry());
            queryString = queryString.Replace("shapes", "triage");

            DataTable queryResultTable = repository.ReadDataFromSpecificQuery(queryString);
            Dictionary<string, List<string>> queryResults = ReadSqlResult(queryResultTable);

            return new QueryResult(query.Id, query.Name, queryResults);
        }

        /// <summary>
        /// Loads all queries from storage.
        /// </summary>
        public void LoadQueries()
        {
            this.Queries = this._queriesRepository.GetAllQueries();
            PointResult.Queries = this.Queries.Values.Where(
                query => PointResult.AddQueryToResult(query)).ToList();
            ShapeResult.Queries = this.Queries.Values.Where(
                query => ShapeResult.AddQueryToResult(query)).ToList();
        }

        /// <summary>
        /// Get the query fields for a specific query.
        /// </summary>
        /// <param name="queryId">Unique query id.</param>
        /// <returns>The query fields or null if not found.</returns>
        /// <exception cref="Exception">Is thrown if Queries are null. At this point, this should not happen
        /// as the queries are loaded before in the controller.</exception>
        public string? GetQueryFields(int queryId)
        {   
            if (this.Queries is null)
            {
                throw new Exception("Queries not loaded.");
            }

            if (!this.Queries.ContainsKey(queryId))
            {
                return null;
            }

            return this.Queries[queryId].QueryFelder;
        }

        /// <summary>
        /// Reads the repository of the sql query. 
        /// </summary>
        /// <param name="dataTable">The datatable containing the data.</param>
        /// <returns>A Dictionary containing the Results of the sql query.</returns>
        private static Dictionary<string, List<string>> ReadSqlResult(DataTable dataTable)
        {
            Dictionary<string, List<string>> queryResults = new();
            int currentRow = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (currentRow == 0)
                    {
                        string fieldName = column.ColumnName;
                        queryResults.Add(fieldName, new List<string>());
                    }
                    string value = (row[column].ToString()) ?? string.Empty;
                    queryResults[column.ColumnName].Add(value);
                }

                currentRow++;
            }

            return queryResults;
        }
    }
}
