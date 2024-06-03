using eBauGISTriageApi.Helper.DTO;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Helper.Functions;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RulesEngine.HelperFunctions;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using YamlDotNet.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace eBauGISTriageApi.Services
{
    /// <summary>
    /// This file contains all functions in relation to the triage.
    /// The core functionality is to check the rules defined in the *Workflows.json files
    /// with the Microsoft Rules Engine.
    /// </summary>
    public class TriageService
    {
        private readonly ILogger _logger;
        private readonly IWorkflowsRepository _workflowsRepository;

        /// <summary>
        /// Gets or sets the list of workflows loaded by the service.
        /// </summary>
        public List<Workflow>? Workflows { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriageService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="workflowsRepository">The repository for accessing workflow data.</param>
        public TriageService(ILogger logger, IWorkflowsRepository workflowsRepository)
        {
            _logger = logger;
            _workflowsRepository = workflowsRepository;
        }

        /// <summary>
        /// Checks the gis Results of the response against all rules.
        /// </summary>
        /// <param name="response">The response object that should be enhanced with the versandbemerkungen.</param>
        /// <param name="dates">The dates passed in by the request to use within the rules engine.</param>
        /// <param name="strings">The strings passed in by the request to use within the rules engine.</param>
        /// <param name="bools">The bools passed in by the request to use within the rules engine.</param>
        /// <param name="ints">The ints passed in by the request to use within the rules engine.</param>
        /// <returns>The response enhanced with versandbemerkungen.</returns>
        /// <exception cref="Exception">The exception is thrown if no workflows are loaded, ergo Workflows equals null.</exception>
        public Response CheckRules(
            Response response,
            Dictionary<string, DateTime>? dates,
            Dictionary<string, string>? strings,
            Dictionary<string, bool>? bools,
            Dictionary<string, int>? ints)
        {
            if (Workflows is null)
            {
                throw new Exception("No workflows loaded.");
            }

            // Get from gis results the transformed query names.  
            HashSet<string> queryNames = response.GisResults
                .SelectMany(gisResult => gisResult
                    .GetQueries()
                    .Select(p => p.Name.ToLower().Replace('-', '_').Replace(' ', '_').Replace('.', '_').Replace(',', '_')))
                .ToHashSet<string>();

            // Empty gis results can not be removed earlier as we need the queries
            // of each type (Point, Shape). 
            for (int i = 0; i < response.GisResults.Count; i++)
            {
                GisResult gisResult = response.GisResults[i];

                if (gisResult.QueryResults.Count <= 0)
                {
                    response.GisResults.RemoveAt(i);
                }
            }

            var queryResults = new Dictionary<string, QueryResult>();

            // Use a deep copy instead of the original list as it changes the values.
            var copiedResponse = JsonConvert.DeserializeObject<Response>(JsonConvert.SerializeObject(response));
            if (copiedResponse is null)
            {
                throw new Exception("Could not create a deep copy of the response.");
            }

            // If multiple gis results have the same query results,
            // the keys should not be duplicated and tested. Therefore, 
            // this code groups them by QueryName and Results.Key.
            copiedResponse.GisResults.ForEach(gisResult =>
            {
                gisResult.QueryResults.ForEach(queryResult =>
                {
                    if (!queryResults.TryAdd(queryResult.QueryName, queryResult))
                    {
                        QueryResult qr = queryResults[queryResult.QueryName];
                        foreach (var result in queryResult.Results)
                        {
                            if (!qr.Results.TryAdd(result.Key, result.Value))
                            {
                                qr.Results[result.Key].AddRange(result.Value);
                            }
                        }
                    }
                });
            });

            var queryResultJObjects = TransformQueryResultsToJObjects(queryResults.Values.ToList());
            var ruleParameters = CreateRuleParameters(dates, strings, bools, ints, queryResultJObjects);
            var activations = ExecuteRules(ruleParameters, queryNames);
            response.Activations.AddRange(activations);

            return response;
        }

        /// <summary>
        /// Loads the workflows through the repository and sets the Workflows property.
        /// </summary>
        public void LoadWorkflows()
        {
            this.Workflows = this._workflowsRepository.GetWorkflows();
        }

        /// <summary>
        /// This function gets the result from the tree recursively. 
        /// It adds all the results from each child to a string. 
        /// This string is the activation remark string.
        /// </summary>
        /// <param name="ruleResultTree">The tree that I want to get the result from.</param>
        /// <param name="queryNames">The query names are used to compare if a rule parameter 
        /// (used to give custom identifier in JSON file), that lead to the exception 'unknown identifier', 
        /// is a query identifier. If yes, do nothing as this is by design, 
        /// if no, an identifier does not exist as rule parameter and throw a CustomException.</param>
        /// <returns>The activation remark string.</returns>
        /// <exception cref="CustomException">Is thrown when a rule has an exception. A rule has an exception 
        /// if the property 'ExceptionMessage' is not null.</exception>
        private string GetActivationRemarkFromRuleResultTree(
            RuleResultTree ruleResultTree,
            HashSet<string> queryNames)
        {
            if (!ruleResultTree.ExceptionMessage.Equals(string.Empty))
            {
                bool known = IsUnknownIdentifierEqualToQueryName(ruleResultTree, queryNames);
                if (!known)
                {
                    ThrowException(
                        ruleResultTree.Rule.RuleName,
                        ruleResultTree.ExceptionMessage,
                        null);
                }
            }

            if (ruleResultTree.ActionResult.Exception != null)
            {
                ThrowException(
                    ruleResultTree.Rule.RuleName,
                    ruleResultTree.ActionResult.Exception.Message,
                    ruleResultTree.ActionResult.Exception.StackTrace);
            }

            string activationRemark = string.Empty;

            if (ruleResultTree.IsSuccess)
            {
                activationRemark += GetChildActivationRemarks(ruleResultTree.ChildResults, queryNames);

                // Only either SuccessEvent or ActionResult can output a result. Both are not supported.
                string remark = ruleResultTree.Rule.SuccessEvent;

                if (ruleResultTree.ActionResult is not null && ruleResultTree.ActionResult.Output is not null)
                {
                    remark = ruleResultTree.ActionResult.Output.ToString() ?? string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    activationRemark += string.IsNullOrWhiteSpace(activationRemark) ? remark : ", " + remark;
                }
            }

            return activationRemark;
        }

        /// <summary>
        /// Handles the child results from the RuleResultTree.
        /// </summary>
        /// <param name="childResults">The child results to handle.</param>
        /// <param name="queryNames">The query names.</param>
        /// <returns>The activation remark string.</returns>
        private string GetChildActivationRemarks(IEnumerable<RuleResultTree> childResults, HashSet<string> queryNames)
        {
            string activationRemarks = string.Empty;

            if (childResults is not null)
            {
                foreach (var child in childResults)
                {
                    string remark = GetActivationRemarkFromRuleResultTree(child, queryNames);
                    if (!string.IsNullOrWhiteSpace(remark))
                    {
                        activationRemarks += string.IsNullOrWhiteSpace(activationRemarks) ? remark : ", " + remark;
                    }
                }
            }

            return activationRemarks;
        }

        /// <summary>
        /// Transforms query results to JObjects to be used as rule parameters. 
        /// </summary>
        /// <param name="queryResults">The query results to transform.</param>
        /// <returns>The transformed query results as jObjects.</returns>
        /// <exception cref="JsonException">Is thrown if the deserialized jObject is null.</exception>
        private static List<JObject> TransformQueryResultsToJObjects(List<QueryResult> queryResults)
        {
            return queryResults
                .Select(queryResult =>
                {
                    var jsonString = JsonConvert.SerializeObject(queryResult);
                    var jObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                    return jObject ?? throw new JsonException("Deserializing queryResult json-string to JObject failed.");
                })
                .ToList();
        }

        /// <summary>
        /// Creates the rule parameters out of the given parameters.
        /// </summary>
        /// <param name="dates">All dates out of the request.</param>
        /// <param name="strings">All strings out of the request.</param>
        /// <param name="bools">All booleans out of the request.</param>
        /// <param name="ints">All integers out of the request.</param>
        /// <param name="queryResultJObjects">The query results as JObjects.</param>
        /// <returns>A list of rule parameters.</returns>
        /// <exception cref="Exception">Is thrown if the query result JObjects are null.</exception>
        private static List<RuleParameter> CreateRuleParameters(
            Dictionary<string, DateTime>? dates,
            Dictionary<string, string>? strings,
            Dictionary<string, bool>? bools,
            Dictionary<string, int>? ints,
            List<JObject> queryResultJObjects)
        {
            if (queryResultJObjects is null)
            {
                throw new Exception("QueryResultJObjects in TriageService.CreateRuleParameters() should not be null.");
            }

            var ruleParameters = new List<RuleParameter>();

            if (dates is not null)
            {
                ruleParameters.AddRange(dates.Select(kvp => new RuleParameter(kvp.Key, kvp.Value)));
            }

            if (strings is not null)
            {
                ruleParameters.AddRange(strings.Select(kvp => new RuleParameter(kvp.Key, kvp.Value)));
            }

            if (bools is not null)
            {
                ruleParameters.AddRange(bools.Select(kvp => new RuleParameter(kvp.Key, kvp.Value)));
            }

            if (ints is not null)
            {
                ruleParameters.AddRange(ints.Select(kvp => new RuleParameter(kvp.Key, kvp.Value)));
            }

            ruleParameters.AddRange(queryResultJObjects.Select(jObject =>
            {
                if (jObject["QueryName"] is null)
                {
                    throw new Exception("QueryResults must have a QueryName property.");
                }

                return new RuleParameter(
                jObject["QueryName"]
                    .ToString()
                    .ToLower()
                    .Replace('-', '_')
                    .Replace(' ', '_')
                    .Replace('.', '_')
                    .Replace(',', '_'),
                jObject);
            }));

            return ruleParameters;
        }

        /// <summary>
        /// Creates a rule engine and executes all rules over all workflows. 
        /// </summary>
        /// <param name="ruleParameters">The rule parameters to be used in the workflow files.</param>
        /// <param name="queryNames">The query names to check the unexpected identifiers.</param>
        /// <returns>A list containing all activations.</returns>
        private List<Activation> ExecuteRules(List<RuleParameter> ruleParameters, HashSet<string> queryNames)
        {
            if (Workflows is null)
            {
                throw new Exception("No workflows loaded.");
            }

            var reSettingsWithCustomTypes = new ReSettings
            {
                CustomTypes = new Type[] { typeof(Helper.Functions.Utils) }
            };

            var rulesEngine = new RulesEngine.RulesEngine(
            Workflows.ToArray(),
            reSettingsWithCustomTypes);

            var activationDictionary = new Dictionary<int, Activation>();

            foreach (var workflow in Workflows)
            {
                var resultList = rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName, ruleParameters.ToArray()).Result;

                var activations = resultList
                    .Select(result => new
                    {
                        FachstellenId = int.Parse(workflow.WorkflowName.Split('_')[0]),
                        ActivationRemark = GetActivationRemarkFromRuleResultTree(result, queryNames)
                    })
                    .Where(activation => !string.IsNullOrEmpty(activation.ActivationRemark))
                    .GroupBy(activation => activation.FachstellenId)
                    .Select(g => new Activation(g.Key, string.Join(", ", g.Select(r => r.ActivationRemark).ToArray())))
                    .ToList();

                foreach (var activation in activations)
                {
                    if (!activationDictionary.ContainsKey(activation.FachstellenId))
                    {
                        activationDictionary.Add(activation.FachstellenId, activation);
                    }
                    else
                    {
                        activationDictionary[activation.FachstellenId].ActivationRemark += ", " + activation.ActivationRemark;
                    }
                }
            }

            return activationDictionary.Values.ToList();
        }

        /// <summary>
        /// Logs information about the error and throws a CustomException.
        /// </summary>
        /// <param name="ruleName">The rule name which lead to an exception.</param>
        /// <param name="message">The message about the exception.</param>
        /// <param name="additionalInfo">Additional information about the error e.g. stacktrace.</param>
        /// <exception cref="CustomException">Is thrown by this function.</exception>
        private void ThrowException(string ruleName, string message, string? additionalInfo)
        {
            this._logger.LogError("Exception while processing rule {rule}", ruleName);
            this._logger.LogError("Detailed error message: {message}", message);
            this._logger.LogError("{info}", additionalInfo);
            throw new CustomException(
                "Exception while processing rule " + ruleName,
                "Detailed error message: " + message);
        }

        /// <summary>
        /// Checks if the given ruleResultTree.ExceptionMessage Unknown identifier is 
        /// a query name.
        /// </summary>
        /// <param name="ruleResultTree">The rule result tree that has thrown the exception.</param>
        /// <param name="queryNames">All query names. </param>
        /// <returns>If unknown identifier is a query name, return true, else false.</returns>
        private static bool IsUnknownIdentifierEqualToQueryName(RuleResultTree ruleResultTree, HashSet<string> queryNames)
        {
            if (ruleResultTree.ExceptionMessage.Contains("Unknown identifier"))
            {
                string unknownIdentifier = ruleResultTree.ExceptionMessage
                    .Substring(ruleResultTree.ExceptionMessage.IndexOf("Unknown identifier"));

                string identifier = unknownIdentifier
                    .Substring(unknownIdentifier.IndexOf("'") + 1, unknownIdentifier.LastIndexOf("'") - unknownIdentifier.IndexOf("'") - 1);

                if (queryNames.Contains(identifier))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
