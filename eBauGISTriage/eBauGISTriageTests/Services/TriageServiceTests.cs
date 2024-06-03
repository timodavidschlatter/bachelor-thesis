using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using eBauGISTriageApi.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using RulesEngine.Models;
using System.Text.Json;
using Xunit;

namespace eBauGISTriageUnitTests.Services
{
    /// <summary>
    /// This class contains all unit tests for the TriageService.
    /// </summary>
    public class TriageServiceTests
    {
        [Fact]
        public void CheckRules_GisResultForActivation_ShouldReturnOneActivation()
        {
            // Arrange
            var logger = new LoggerFactory().CreateLogger<TriageService>();
            var workflowsRepository = new MockWorkflowsRepository();
            var service = new TriageService(logger, workflowsRepository);
            service.LoadWorkflows();

            var response = new Response();
            var pointResultJsonString = "{\"queryResults\":[{\"queryName\":\"LZE-NL-ZPS_Landwirtschaftszone\",\"results\":{\"wert\":[\"grenzt an ZPL\"]}}]}";
            var pointResult = JsonSerializer.Deserialize<PointResult>(pointResultJsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, });
            Assert.NotNull(pointResult);

            PointResult.Queries = new();
            response.GisResults.Add(pointResult);

            // Act
            var result = service.CheckRules(response, null, null, null, null);

            // Assert
            Assert.NotEmpty(result.Activations);
            Assert.Single(result.Activations);
            Assert.Equal(305, result.Activations[0].FachstellenId);
            Assert.Equal("grenzt an ZPL", result.Activations[0].ActivationRemark);
        }

        /// <summary>
        /// Mock implementation of IWorkflowsRepository for testing.
        /// </summary>
        private class MockWorkflowsRepository : IWorkflowsRepository
        {
            public List<Workflow> GetWorkflows()
            {
                // Return sample workflows for testing
                var rule = new Rule();
                rule.RuleName = "LZE-NL-ZPS_Landwirtschaftszone";
                rule.RuleExpressionType = RuleExpressionType.LambdaExpression;
                rule.Expression = "lze_nl_zps_landwirtschaftszone.QueryName == \"LZE-NL-ZPS_Landwirtschaftszone\" && Utils.ResultExists(lze_nl_zps_landwirtschaftszone.Results.wert)";
                rule.Actions = new RuleActions
                {
                    OnSuccess = new()
                };
                rule.Actions.OnSuccess.Context = new();
                rule.Actions.OnSuccess.Name = "OutputExpression";
                rule.Actions.OnSuccess.Context.Add("Expression", "lze_nl_zps_landwirtschaftszone.Results.wert[0]");
                var rules = new List<Rule>();
                rules.Add(rule);

                var workflow = new Workflow();
                workflow.WorkflowName = "305_LZE-NL";
                workflow.Rules = rules;

                var workflows = new List<Workflow>();
                workflows.Add(workflow);

                return workflows;
            }
        }
    }
}
