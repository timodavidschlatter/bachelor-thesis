using BUDSharedCore.Persistence.Context;
using eBauGISTriageApi;
using eBauGISTriageApi.Helper.DTO;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using eBauGISTriageApi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using Xunit.Abstractions;

namespace eBauGISTriageIntegrationTests
{
    /// <summary>
    /// Represents a class containing performance tests for the eBauGISTriageApi.
    /// </summary>
    public class PerformanceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;
        private readonly Request? _request;
        private readonly GeometryDTO _geometryDTO;
        private readonly Response _response;
        private readonly QueriesService? _queriesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceTests"/> class.
        /// </summary>
        /// <param name="factory">The web application factory.</param>
        /// <param name="output">The test output helper.</param>
        public PerformanceTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            this._factory = factory;
            this._output = output;
            string jsonRequestBody = "" +
                "{\"shapes\":[\"POLYGON((2627973.6339951 1254304.548002,2627980.2419907 1254307.8530007," +
                "2627988.3290024 1254291.5920022,2627980.937999 1254288.1130044,2627975.0249985 1254299.6790017," +
                "2627975.9809922 1254299.7660012,2627973.6339951 1254304.548002))\"]," +
                "\"points\":[\"POINT(2627981.111998 1254296.9829995)\"]}";

            using var scope = this._factory.Services.CreateScope();
            this._queriesService = scope.ServiceProvider.GetService<QueriesService>();
            this._request = JsonSerializer.Deserialize<Request>(
                    jsonRequestBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ValidationContext context = new(this._request);
            this._request.Validate(context);
            this._geometryDTO = new(
                this._request.PointResults,
                this._request.ShapeResult);
            this._response = new();
        }

        /// <summary>
        /// Tests the ParallelExecuteQueries() (multithreaded) method on its execution time to compare
        /// with the singlethreaded execution time. 
        /// </summary>
        [Fact]
        public void ParallelExecuteQueries_Performance_Pin()
        {
            Assert.NotNull(this._queriesService);

            // Act
            this._queriesService.LoadQueries();
            TimeSpan timeSpan = this.Time(() =>
            this._queriesService.ParallelExecuteQueries(_response, _geometryDTO));

            // Assert
            this._output.WriteLine("Time: " + timeSpan);
            Assert.True(timeSpan < TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Tests the ParallelExecuteQueries() (multithreaded) method on its execution time to compare
        /// with the singlethreaded execution time. 
        /// </summary>
        [Fact]
        public void ExecuteQuery_Performance_Pin()
        {
            Assert.NotNull(this._queriesService);
            this._queriesService.LoadQueries();
            Assert.NotNull(this._queriesService.Queries);

            TimeSpan timeSpan = this.Time(() =>
            {
                // Arrange
                this._response.GisResults.AddRange(this._geometryDTO.PointResults);
                this._response.GisResults.Add(this._geometryDTO.ShapeResult);
                var configuration = new ConfigurationBuilder()
                    .AddUserSecrets<PerformanceTests>()
                    .Build();
                var serviceProvider = new ServiceCollection()
                    .AddDbContext<GdwhCtx>(options => options.UseNpgsql(configuration["ConnectionStrings:GDWHConnection"]))
                    .AddSingleton(new DbContextConfig<GdwhCtx>(string.Empty))
                    .BuildServiceProvider();
                var gdwhCtx = serviceProvider.GetRequiredService<GdwhCtx>();
                var loggerMock = new Mock<ILogger>();
                IGdwhRepository repository = new GdwhRepository(gdwhCtx, loggerMock.Object);

                // Act
                for (int i = 0; i < this._response.GisResults.Count; i++)
                {
                    GisResult gisResult = this._response.GisResults[i];
                    foreach (Query query in gisResult.GetQueries())
                    {
                        QueryResult queryResult = this._queriesService.ExecuteQuery(query, gisResult, repository);
                        if (queryResult.Results.Count > 0)
                        {
                            gisResult.QueryResults.Add(queryResult);
                        }
                    }
                    if (gisResult.QueryResults.Count <= 0)
                    {
                        this._response.GisResults.Remove(gisResult);
                    }
                }
            });
            
            // Assert
            this._output.WriteLine("Time: " + timeSpan);
            Assert.True(timeSpan < TimeSpan.FromSeconds(20));
        }

        /// <summary>
        /// Measures the time of executing the method ParallelExecuteQueries
        /// without loading all the queries.
        /// </summary>
        [Fact]
        public void ExecuteQueriesWithoutLoadingQueriesPerRequest_Performance_Pin()
        {
            Assert.NotNull(this._queriesService);

            // Act
            this._queriesService.LoadQueries();
            TimeSpan timeSpan = this.Time(() =>
                this._queriesService.ParallelExecuteQueries(_response, _geometryDTO));

            // Assert
            this._output.WriteLine("Time: " + timeSpan);
            Assert.True(timeSpan < TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Measures the time of executing the method ParallelExecuteQueries
        /// and also loading all the queries.
        /// </summary>
        [Fact]
        public void ExecuteQueriesWithLoadingQueriesPerRequest_Performance_Pin()
        {
            Assert.NotNull(this._queriesService);

            // Act
            TimeSpan timeSpan = this.Time(() =>
            {
                this._queriesService.LoadQueries();
                this._queriesService.ParallelExecuteQueries(_response, _geometryDTO);
            });

            // Assert
            this._output.WriteLine("Time: " + timeSpan);
            Assert.True(timeSpan < TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Measure the time of a method.
        /// Source: https://stackoverflow.com/questions/15181358/how-can-i-unit-test-performance-optimisations-in-c
        /// </summary>
        /// <param name="toTime">The method to time.</param>
        /// <returns>The elapsed time.</returns>
        private TimeSpan Time(Action toTime)
        {
            Stopwatch timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}
