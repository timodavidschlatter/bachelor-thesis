using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using eBauGISTriageApi.Helper.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents a request object used in the GIS triage process.
    /// </summary>
    public class Request : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the list of shape strings in the request.
        /// </summary>
        [Required]
        public List<string> Shapes { get; set; }

        /// <summary>
        /// Gets or sets the list of point strings in the request.
        /// </summary>
        [Required]
        public List<string> Points { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of dynamic date objects used with the rules engine.
        /// </summary>
        public Dictionary<string, DateTime>? Dates { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of dynamic string objects used with the rules engine.
        /// </summary>
        public Dictionary<string, string>? Strings { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of dynamic bool objects used with the rules engine.
        /// </summary>
        public Dictionary<string, bool>? Booleans { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of dynamic int objects used with the rules engine.
        /// </summary>
        public Dictionary<string, int>? Integers { get; set; }

        /// <summary>
        /// Gets or sets the shape result object.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public ShapeResult? ShapeResult { get; set; }

        /// <summary>
        /// Gets or sets the list of point result objects.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<PointResult>? PointResults { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="shapes">The list of shape strings in the request.</param>
        /// <param name="points">The list of point strings in the request.</param>
        /// <param name="dates">The dictionary of dynamic date objects used with the rules engine.</param>
        /// <param name="strings">The dictionary of dynamic string objects used with the rules engine.</param>
        /// <param name="booleans">The dictionary of dynamic bool objects used with the rules engine.</param>
        /// <param name="integers">The dictionary of dynamic int objects used with the rules engine.</param>
        public Request(
            List<string> shapes, 
            List<string> points, 
            Dictionary<string, DateTime> dates, 
            Dictionary<string, string> strings, 
            Dictionary<string, bool> booleans, 
            Dictionary<string, int> integers)
        {
            Shapes = shapes;
            Points = points;
            Dates = dates;
            Strings = strings;
            Booleans = booleans;
            Integers = integers;
        }

        /// <summary>
        /// Validates the request object.
        /// It checks if the point strings really contains two coordinates.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            try
            {
                JArray triageArray = new(this.Shapes);
                JObject jObject = new()
                {
                    ["shapes"] = triageArray,
                };
                this.ShapeResult = new(jObject.ToString(Formatting.None));

                this.PointResults = new();
                this.Points.ForEach(pointString =>
                {
                    string[] coordinates = pointString.Split('(', ')')[1].Split(' ');
                    Point point = new Point(
                        double.Parse(coordinates[0]),
                        double.Parse(coordinates[1]));
                    this.PointResults.Add(new(point));
                });
            }
            catch (Exception ex) when (ex is FormatException || ex is IndexOutOfRangeException)
            {
                results.Add(new ValidationResult("Not valid point format in points", new[] { nameof(this.Points) }));
            }
            catch (Exception ex)
            {
                results.Add(new ValidationResult("Unexpected error while validating the request object.\n" +
                    "Error: " + ex.Message));
            }

            return results;
        }
    }
}
