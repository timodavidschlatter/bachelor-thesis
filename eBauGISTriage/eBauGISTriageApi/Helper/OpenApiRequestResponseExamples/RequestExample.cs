using eBauGISTriageApi.Helper.DTO;
using eBauGISTriageApi.Models;
using NSwag.Examples;
using System;
using System.Collections.Generic;

namespace eBauGISTriageApi.Helper.OpenApiRequestResponseExamples
{
    /// <summary>
    /// Represents a request example used in the OpenAPI documentation.
    /// </summary>
    public class RequestExample : IExampleProvider<Request>
    {
        /// <summary>
        /// Gets an example of a Request object.
        /// </summary>
        /// <returns>An example Request object.</returns>
        public Request GetExample()
        {
            List<string> shapes = new()
            {
                "POLYGON((2627973.6339951 1254304.548002,2627980.2419907 1254307.8530007,2627988.3290024 1254291.5920022,2627980.937999 1254288.1130044,2627975.0249985 1254299.6790017,2627975.9809922 1254299.7660012,2627973.6339951 1254304.548002))",
            };
            List<string> points = new()
            {
                "POINT(2627981.111998 1254296.9829995)",
            };

            Dictionary<string, DateTime> dates = new()
            {
                { "baugesuch_geschaeftsdatum", new DateTime(2018, 6, 11) }
            };

            Dictionary<string, string> strings = new()
            {
                { "baugesuch_prj_ort1", "Lausen" }
            };

            Dictionary<string, bool> bools = new()
            {
                { "baugesuch_neuauflage", true }
            };

            Dictionary<string, int> ints = new()
            {
                { "baugesuch_gebauedetyp_id", 210 },
                { "baugesuch_jahr", 2016 },
            };

            return new Request(
                shapes,
                points,
                dates,
                strings,
                bools,
                ints);
        }
    }
}
