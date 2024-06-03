using System;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using eBauGISTriageApi.Helper.DTO;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace eBauGISTriageApi.Controllers
{
    /// <summary>
    /// Represents the TriageController responsible for handling triage-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TriageController : ControllerBase
    {
        private readonly QueriesService _queriesService;
        private readonly TriageService _triageService;
        private readonly ILogger _logger;
        private ProblemDetails _problemDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriageController"/> class.
        /// </summary>
        /// <param name="queriesService">The queries service.</param>
        /// <param name="triageService">The triage service.</param>
        /// <param name="logger">The logger.</param>
        public TriageController(QueriesService queriesService, 
            TriageService triageService,
            ILogger<TriageController> logger)
        {
            this._queriesService = queriesService;
            this._triageService = triageService;
            this._logger = logger;
            this._problemDetails = new ValidationProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
            };
        }

        /// <summary>
        /// Execute the gis triage.
        /// </summary>
        /// <param name="request">The request object. It contains shapes, points and additional params 
        /// like e.g. strings to be used within the rules engine.</param>
        /// <returns>Returns a response with two lists containing all gis Results and all activation remarks.</returns>
        /// <response code="200">Returns a response with two lists containing all gis Results and all activation remarks.</response>
        /// <response code="400">Invalid JSON string according to the transmitted schema in the post body.</response>
        /// <response code="406">Not supported "Accept" header.</response>
        /// <response code="415">Not supported "Content-Type" header.</response>
        /// <response code="500">Internal server error (e.g. No connection to database).</response>
        [HttpPost]
        [Route("startTriage")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult StartTriage([FromBody] Request request)
        {
            if (!this.Request.Headers["Accept"].Any(a =>
            a.Equals(MediaTypeNames.Application.Json) ||
            a.Equals("application/*") ||
            a.Equals("*/*")))
            {
                return this.StatusCode(406);
            }

            this._logger.LogInformation("Start processing the request in TriageController.StartTriage().");

            try
            {
                if (request.PointResults is null)
                {
                    return BadRequest("PointResults cannot be null.");
                }

                if (request.ShapeResult is null)
                {
                    return BadRequest("ShapeResult cannot be null.");
                }

                if (this._queriesService.Queries is null)
                {
                    this._queriesService.LoadQueries();
                }

                if (this._triageService.Workflows is null)
                {
                    this._triageService.LoadWorkflows();
                }

                GeometryDTO geometryDTO = new GeometryDTO(
                    request.PointResults, 
                    request.ShapeResult);

                Response? response = new();
                response = this._queriesService.ParallelExecuteQueries(
                    response,
                    geometryDTO
                    );
                response = this._triageService.CheckRules(
                    response, 
                    request.Dates, 
                    request.Strings,
                    request.Booleans,
                    request.Integers);

                return this.Ok(response);
            }
            catch (CustomException ex)
            {
                return HandleException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Returns the query fields of a specific query.
        /// </summary>
        /// <param name="id">Unique query id.</param>
        /// <returns>The query fields as string.</returns>
        [HttpGet]
        [Route("getQueryFields")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult GetQueryFields(int id)
        {
            try
            {
                this._logger.LogInformation("Get query fields.");

                if (this._queriesService.Queries is null)
                {
                    this._queriesService.LoadQueries();
                }

                string? queryFields = this._queriesService.GetQueryFields(id);

                if (queryFields is null)
                {
                    return NotFound();
                }

                return Ok(queryFields);
            }
            catch (CustomException ex)
            {
                return HandleException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Manually reload the queries of the queries folder.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Queries were successfully loaded.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Route("loadQueries")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult LoadQueries()
        {
            try
            {
                this._logger.LogInformation("Loading queries.");
                this._queriesService.LoadQueries();
                return this.NoContent();
            }
            catch (CustomException ex)
            {
                return HandleException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Manually reload the Workflows.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Workflows were successfully loaded.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Route("loadWorkflows")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult LoadWorkflows()
        {
            try
            {
                this._logger.LogInformation("Loading workflows.");
                this._triageService.LoadWorkflows();
                return this.NoContent();
            }
            catch (CustomException ex)
            {
                return HandleException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Logs exception details and returns the ProblemDetails object.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        /// <returns>ProblemDetails object containing information about the error.</returns>
        private IActionResult HandleException(Exception ex)
        {
            this._logger.LogError("Unexpected exception.");
            this._logger.LogError("Detailed error message: {message}", ex.Message);
            this._logger.LogError(ex.StackTrace);
            _problemDetails.Detail = ex.Message;
            _problemDetails.Instance = HttpContext.Request.Path;
            return StatusCode(StatusCodes.Status500InternalServerError, _problemDetails);
        }

        /// <summary>
        /// Logs exception details and returns the ProblemDetails object.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        /// <returns>ProblemDetails object containing information about the error.</returns>
        private IActionResult HandleException(CustomException ex)
        {
            this._logger.LogError("Detailed error message: {message}", ex.Message);
            this._logger.LogError("Additional information: {information}", ex.AdditionalInfo);
            this._logger.LogError(ex.StackTrace);
            _problemDetails.Detail = ex.Message;
            _problemDetails.Extensions.Add("additionalInfo", ex.AdditionalInfo);
            _problemDetails.Instance = HttpContext.Request.Path;
            return StatusCode(StatusCodes.Status500InternalServerError, _problemDetails);
        }
    }
}
