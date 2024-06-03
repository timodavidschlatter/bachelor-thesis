using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Examples;

namespace eBauGISTriageApi.Helper.OpenApiRequestResponseExamples
{
    /// <summary>
    /// Represents an example of the ValidationProblemDetails used in the OpenAPI documentation.
    /// </summary>
    public class ValidationProblemDetailsExample : IExampleProvider<ValidationProblemDetails>
    {
        /// <summary>
        /// Gets an example of a ValidationProblemDetails object.
        /// </summary>
        /// <returns>An example ValidationProblemDetails object.</returns>
        public ValidationProblemDetails GetExample()
        {
            ValidationProblemDetails validationProblemDetails = new()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
            };
            validationProblemDetails.Errors.Add("Shapes", new string[] { "The Shapes field is required." });
            return validationProblemDetails;
        }
    }
}
