using System;
using System.Collections.Generic;
using System.Linq;

namespace eBauGISTriageApi.Models
{
    /// <summary>
    /// Represents an activation.
    /// </summary>
    public class Activation
    {
        /// <summary>
        /// Gets the FachstellenId.
        /// </summary>
        public int FachstellenId { get; }

        /// <summary>
        /// Gets or sets the activation remark.
        /// </summary>
        public string ActivationRemark { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activation"/> class.
        /// </summary>
        /// <param name="fachstellenId">The FachstellenId.</param>
        /// <param name="activationRemark">The activation remark.</param>
        public Activation(int fachstellenId, string activationRemark)
        {
            this.FachstellenId = fachstellenId;
            this.ActivationRemark = activationRemark;
        }

        public override bool Equals(object? obj)
        {
            return obj is Activation activation &&
                   FachstellenId == activation.FachstellenId &&
                   this.ActivationRemark.Equals(activation.ActivationRemark);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.FachstellenId, this.ActivationRemark);
        }
    }
}
