using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// Represents a repository for accessing and retrieving queries from the filesystem.
    /// It uses the file system as storage.
    /// </summary>
    public class QueriesRepository : IQueriesRepository
    {
        private readonly ILogger _logger;
        private readonly string _pathToQueriesFolder;
        private readonly IFileSystem _fileSystem;
        private const string c_queryFilePath = "Query.sql";
        private const string c_queryDetailsFilePath = "QueryDetails.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="QueriesRepository"/> class.
        /// </summary>
        /// <param name="pathToQueries">The path to the queries folder.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="fileSystem">The file system abstraction.</param>
        public QueriesRepository(string pathToQueries, ILogger logger, IFileSystem fileSystem)
        {
            this._pathToQueriesFolder = pathToQueries;
            this._logger = logger;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Reads from the filesystem and returns all existing queries.
        /// </summary>
        /// <returns>A dictionary with the query id as key and the query object as value.</returns>
        /// <exception cref="CustomException"></exception>
        public Dictionary<int, Query> GetAllQueries()
        {
            string? currentDirectory = null; // Needed for the exception message.

            try
            {
                Dictionary<int, Query> queries = new();
                List<string> directories = new(_fileSystem.Directory.EnumerateDirectories(_pathToQueriesFolder));

                directories.ForEach(dir =>
                {
                    currentDirectory = dir;

                    Query? query = JsonConvert.DeserializeObject<Query>(
                        _fileSystem.File.ReadAllText(dir + "/" + c_queryDetailsFilePath),
                        new JsonSerializerSettings
                        {
                            ContractResolver = new DefaultContractResolver
                            {
                                // Enable attribute-based validation
                                NamingStrategy = new CamelCaseNamingStrategy(),
                                SerializeCompilerGeneratedMembers = true,
                                IgnoreSerializableAttribute = true,
                                IgnoreSerializableInterface = true,
                                IgnoreIsSpecifiedMembers = false,
                                IgnoreShouldSerializeMembers = false,
                            },
                            MissingMemberHandling = MissingMemberHandling.Error // Throw an exception if required members are missing
                        }
                    );

                    query.QueryPostGIS = _fileSystem.File.ReadAllText(dir + "/" + c_queryFilePath);
                    queries.Add(query.Id, query);
                });

                return queries;
            }
            catch (DirectoryNotFoundException ex)
            {
                this._logger.LogError("Queries directory not found.");
                this._logger.LogError("Path to queries folder: {pathToFolder} ", this._pathToQueriesFolder);
                this._logger.LogError("Problem directory: {directory}", currentDirectory);
                this._logger.LogError("Error message: {message}", ex.Message);
                throw new CustomException("DirectoryNotFoundException", "Queries directory not found.");
            }
            catch (Exception ex) when (
            ex is JsonException ||
            ex is ArgumentNullException ||
            ex is NotSupportedException)
            {
                this._logger.LogError("Problem with deserializing the file QueryDetails.json.");
                this._logger.LogError("Problem directory: {directory}", currentDirectory);
                this._logger.LogError("Error message: {message}", ex.Message);
                throw new CustomException("Problem with deserializing the file QueryDetails.json.", "Problem directory: " + currentDirectory);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error: {message}", ex.Message);
                throw new CustomException("Unexpected error", ex.Message);
            }
        }
    }
}
