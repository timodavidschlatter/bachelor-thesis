using BUDSharedCore.Persistence.Context;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eBauGISTriageUnitTests.Persistence
{
    /// <summary>
    /// This class contains all unit tests for the GdwhRepository.
    /// </summary>
    public class GdwhRepositoryTests
    {
        [Fact]
        public void ReadDataFromSpecificQuery_InvalidQuery_ThrowsNpgsqlException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<GdwhRepositoryTests>()
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddDbContext<GdwhCtx>(options => options.UseNpgsql(configuration["ConnectionStrings:GDWHConnection"]))
                .AddSingleton(new DbContextConfig<GdwhCtx>(string.Empty))
                .BuildServiceProvider();
            var gdwhCtx = serviceProvider.GetRequiredService<GdwhCtx>();
            var loggerMock = new Mock<ILogger>();
            var repository = new GdwhRepository(gdwhCtx, loggerMock.Object);

            var invalidQuery = "SELECT * FROM InvalidTable";

            // Act & Assert
            var exception = Assert.Throws<CustomException>(
                () => repository.ReadDataFromSpecificQuery(invalidQuery));
            Assert.NotNull(exception);
            Assert.Equal("NpgsqlException", exception.Message);
            Assert.Equal("Unexpected database error.", exception.AdditionalInfo);
        }
    }
}
