using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using eBauGISTriageApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eBauGISTriageUnitTests.Services
{
    /// <summary>
    /// This class contains all unit tests for the QueriesService.
    /// </summary>
    public class QueriesServiceTests
    {
        [Fact]
        public void GetQueryFields_ReturnsQueryFields_IfExists()
        {
            // Arrange
            var dbContextFactoryMock = new Mock<IDbContextFactory<GdwhCtx>>();
            var loggerMock = new Mock<ILogger<QueriesService>>();
            var queriesRepositoryMock = new Mock<IQueriesRepository>();
            var service = new QueriesService(dbContextFactoryMock.Object, loggerMock.Object, queriesRepositoryMock.Object);
            var queries = new Dictionary<int, Query>
            {
                { 1, new Query(1, "Query1", "PostGIS1", "Field1") },
                { 2, new Query(2, "Query2", "PostGIS2", "Field2") },
            };
            service.Queries = queries;
            int queryId = 1;

            // Act
            var result = service.GetQueryFields(queryId);

            // Assert
            Assert.Equal("Field1", result);
        }

        [Fact]
        public void GetQueryFields_ReturnsNull_IfNotExists()
        {
            // Arrange
            var dbContextFactoryMock = new Mock<IDbContextFactory<GdwhCtx>>();
            var loggerMock = new Mock<ILogger<QueriesService>>();
            var queriesRepositoryMock = new Mock<IQueriesRepository>();
            var service = new QueriesService(dbContextFactoryMock.Object, loggerMock.Object, queriesRepositoryMock.Object);
            var queries = new Dictionary<int, Query>
            {
                { 1, new Query(1, "Query1", "PostGIS1", "Field1") },
                { 2, new Query(2, "Query2", "PostGIS2", "Field2") },
            };
            service.Queries = queries;

            int queryId = 3; // Non-existent query ID

            // Act
            var result = service.GetQueryFields(queryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetQueryFields_ThrowsException_IfQueriesNotLoaded()
        {
            // Arrange
            var dbContextFactoryMock = new Mock<IDbContextFactory<GdwhCtx>>();
            var loggerMock = new Mock<ILogger<QueriesService>>();
            var queriesRepositoryMock = new Mock<IQueriesRepository>();
            var service = new QueriesService(dbContextFactoryMock.Object, loggerMock.Object, queriesRepositoryMock.Object);

            int queryId = 1;

            // Act & Assert
            Assert.Throws<Exception>(() => service.GetQueryFields(queryId));
        }
    }
}
