using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Persistence;
using Microsoft.Extensions.Logging;
using Moq;
using RulesEngine.Models;
using Xunit;

namespace eBauGISTriageUnitTests.Persistence
{
    /// <summary>
    /// This class contains all unit tests for the FileSystemWorkflowsRepository.
    /// </summary>
    public class FileSystemWorkflowsRepositoryTests
    {
        [Fact]
        public void GetWorkflows_WithValidData_ReturnsWorkflows()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var fileSystemMock = new Mock<IFileSystem>();

            // Set up file system mock to return a sample file path and data
            var filePath = "path/to/workflows/Workflow.json";
            var fileData = @"[{""WorkflowName"": ""Workflow 1"", ""WorkflowsToInject"": [], ""GlobalParams"": [], ""Rules"": []},
                              {""WorkflowName"": ""Workflow 2"", ""WorkflowsToInject"": [], ""GlobalParams"": [], ""Rules"": []}]";
            fileSystemMock.Setup(fs => fs.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>()))
                .Returns(new[] { filePath });
            fileSystemMock.Setup(fs => fs.File.ReadAllText(filePath)).Returns(fileData);

            var repository = new FileSystemWorkflowsRepository(loggerMock.Object, "path/to/workflows", fileSystemMock.Object);

            // Act
            var workflows = repository.GetWorkflows();

            // Assert
            Assert.NotNull(workflows);
            Assert.Equal(2, workflows.Count);
            Assert.Equal("Workflow 1", workflows[0].WorkflowName);
            Assert.Equal("Workflow 2", workflows[1].WorkflowName);
        }

        [Fact]
        public void GetWorkflows_WithNoFiles_ThrowsIOException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var fileSystemMock = new Mock<IFileSystem>();
            var filePath = "path/to/workflows";

            // Set up file system mock to return no files
            fileSystemMock.Setup(fs => fs.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>()))
                .Returns(Array.Empty<string>());

            var repository = new FileSystemWorkflowsRepository(loggerMock.Object, filePath, fileSystemMock.Object);

            // Act & Assert
            var exception = Assert.Throws<CustomException>(() => repository.GetWorkflows());
            Assert.Equal("Rules not found.", exception.Message);
            Assert.Equal("Used Workflow path:" + filePath + ", file name: *Workflow.json", exception.AdditionalInfo);
        }
    }
}