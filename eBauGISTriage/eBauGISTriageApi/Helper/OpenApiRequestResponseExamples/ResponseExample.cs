using eBauGISTriageApi.Models;
using NSwag.Examples;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eBauGISTriageApi.Helper.OpenApiRequestResponseExamples
{
    /// <summary>
    /// Represents a response example used in the OpenAPI documentation.
    /// </summary>
    public class ResponseExample : IExampleProvider<Response>
    {
        /// <summary>
        /// Gets an example of a Response object.
        /// </summary>
        /// <returns>An example Response object.</returns>
        public Response GetExample()
        {
            Response response = new();

            PointResult pointResult = new(new Point(2627981.111998, 1254296.9829995));
            Dictionary<string, List<string>> dictionary = new()
            {
                { "wrg", new List<string>() }
            };
            QueryResult queryResult = new(72, "Wildruhegebiet", dictionary);
            pointResult.QueryResults.Add(queryResult);
            response.GisResults.Add(pointResult);

            ShapeResult shapeResult = new("{\"shapes\":[\"POLYGON((2627973.6339951 1254304.548002, 2627980.2419907 1254307.8530007,2627988.3290024 1254291.5920022,2627980.9379991254288.1130044,2627975.0249985 1254299.6790017,2627975.9809922 1254299.7660012,2627973.6339951 1254304.548002))\"]}");
            dictionary = new()
            {
                { "resvalue", new List<string>() { "W gering, W erheblich, W mittel" } }
            };
            queryResult = new(201, "BGV-ESP-Naturgefahren", dictionary);
            shapeResult.QueryResults.Add(queryResult);
            response.GisResults.Add(shapeResult);

            Activation activation = new(437, "im prov. Gewässerraum: Innerhalb Bauzone");
            activation.ActivationRemark += ", autom. Triage: Wildruhegebiet";
            response.Activations.Add(activation);
            return response;
        }
    }
}
