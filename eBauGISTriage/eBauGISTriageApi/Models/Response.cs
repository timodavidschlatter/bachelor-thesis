using System;
using System.Collections.Generic;
using System.Linq;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents the response containing GIS results and activations.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets or sets the list of GIS results.
        /// </summary>
        public List<GisResult> GisResults { get; set; }

        /// <summary>
        /// Gets or sets the list of activations.
        /// </summary>
        public List<Activation> Activations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            this.GisResults = new List<GisResult>();
            this.Activations = new List<Activation>();
        }

        public override bool Equals(object? obj)
        {
            return obj is Response response &&
                    this.GisResults.SequenceEqual(response.GisResults) &&
                    this.Activations.SequenceEqual(response.Activations);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.GisResults, this.Activations);
        }
    }
}
