using eBauGISTriageApi.Helper.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// This file contains the implementation WorkflowsRepository.
    /// It uses the file system as storage.
    /// </summary>
    public class FileSystemWorkflowsRepository : IWorkflowsRepository
    {
        private readonly ILogger _logger;
        private readonly string _pathToWorkflowsFolder;
        private readonly IFileSystem _fileSystem;
        private const string c_fileName = "*Workflow.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWorkflowsRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used for logging.</param>
        /// <param name="pathToWorkflowsFolder">The path to the workflows folder.</param>
        /// <param name="fileSystem">The file system implementation for accessing the file system.</param>
        public FileSystemWorkflowsRepository(
            ILogger logger, 
            string pathToWorkflowsFolder, 
            IFileSystem fileSystem)
        {
            _logger = logger;
            _pathToWorkflowsFolder = pathToWorkflowsFolder;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// <see cref="IWorkflowsRepository.GetWorkflows"/>
        /// This implementation uses the file system as storage.
        /// </summary>
        /// <returns><see cref="IWorkflowsRepository.GetWorkflows"/></returns>
        /// <exception cref="IOException">Is thrown when the read files are null or the length is 0.</exception>
        /// <exception cref="JsonException">Is thrown when the deserialized workflows are null or count is 0.</exception>
        public List<Workflow> GetWorkflows()
        {
            string? currentFile = null; // Needed for the exception message.
            try
            {
                var files = this._fileSystem.Directory.GetFiles(
                    this._pathToWorkflowsFolder,
                    c_fileName,
                    SearchOption.AllDirectories);

                if (files == null || files.Length == 0)
                {
                    throw new IOException("Rules not found.");
                }

                List<Workflow> workflows = new();
                foreach (var file in files)
                {
                    currentFile = file;
                    var fileData = this._fileSystem.File.ReadAllText(file);
                    var tempWorkflows = JsonConvert.DeserializeObject<List<Workflow>>(fileData);

                    if (tempWorkflows is null || tempWorkflows.Count == 0)
                    {
                        throw new JsonException("Workflows are not deserializable.");
                    }

                    workflows.AddRange(tempWorkflows);
                }

                return workflows;
            }
            catch (DirectoryNotFoundException ex)
            {
                this._logger.LogError("Workflows directory not found.");
                this._logger.LogError("Given workflows folder path: {pathToFolder} ", this._pathToWorkflowsFolder);
                this._logger.LogError("Error message: {message}", ex.Message);
                throw new CustomException(
                    "Workflows directory not found.",
                    "Given workflows folder path: " + _pathToWorkflowsFolder);
            }
            catch (Exception ex) when (
            ex is JsonException ||
            ex is ArgumentNullException ||
            ex is NotSupportedException)
            {
                this._logger.LogError("Problem with deserializing the file: {file}", currentFile);
                this._logger.LogError("Error message: {message}", ex.Message);
                throw new CustomException("Problem with deserializing the file: " + currentFile);
            }
            catch (IOException ex)
            {
                this._logger.LogError("Error message: {message}", ex.Message);
                this._logger.LogError(
                    "Used Workflow path: {path}, file name: {fileName}", 
                    this._pathToWorkflowsFolder, 
                    c_fileName);
                throw new CustomException(
                    ex.Message, 
                    "Used Workflow path:" + _pathToWorkflowsFolder + ", file name: " + c_fileName);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error: {message}", ex.Message);
                throw new CustomException("Unexpected error", ex.Message);
            }
        }
    }
}
