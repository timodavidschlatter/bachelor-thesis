using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Persistence;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace eBauGISTriageUnitTests.Persistence
{
    /// <summary>
    /// This class contains all unit tests for the QueriesRepository.
    /// </summary>
    public class QueriesRepositoryTests
    {
        [Fact]
        public void GetAllQueries_DirectoryNotFoundExceptionThrown_ShouldThrowCustomException()
        {
            // Arrange
            var pathToQueries = "NonExistentDirectory";
            var logger = new LoggerFactory().CreateLogger<QueriesRepository>();
            Mock<IFileSystem> fileSystemMock = new();
            fileSystemMock.Setup(fs => fs.Directory.EnumerateDirectories(It.IsAny<string>()))
                .Throws(new DirectoryNotFoundException());
            var repository = new QueriesRepository(pathToQueries, logger, fileSystemMock.Object);

            // Act and Assert
            var exception = Assert.Throws<CustomException>(() => repository.GetAllQueries());
            Assert.Equal("Queries directory not found.", exception.AdditionalInfo);
        }

        [Fact]
        public void GetAllQueries_InvalidFileContent_ThrowsJsonException()
        {
            // Arrange
            Mock<IFileSystem> fileSystemMock = new();
            Mock<IFile> fileMock = new();

            fileSystemMock.Setup(fs => fs.Directory.EnumerateDirectories(It.IsAny<string>()))
                .Returns(new List<string> { "directory1", "directory2" });
            fileSystemMock.Setup(fs => fs.File.ReadAllText(It.IsAny<string>()))
                .Returns("Not valid json content");

            QueriesRepository repository = new QueriesRepository("path/to/queries", Mock.Of<ILogger>(), fileSystemMock.Object);

            // Act & Assert
            var exception = Assert.Throws<CustomException>(() => repository.GetAllQueries());
            Assert.Equal("Problem with deserializing the file QueryDetails.json.", exception.Message);
        }
    }
}
