using System;

namespace eBauGISTriageApi.Helper.Exceptions
{
    /// <summary>
    /// Represents a custom exception used in the application to 
    /// give the user of the service a convenient error message.
    /// </summary>
    public class CustomException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CustomException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class with a specified error message and additional information.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="additionalInfo">Additional information about the exception.</param>
        public CustomException(string message, string additionalInfo)
            : base(message)
        {
            AdditionalInfo = additionalInfo;
        }

        /// <summary>
        /// Gets additional information about the exception.
        /// </summary>
        public string? AdditionalInfo { get; }
    }
}
