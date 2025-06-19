using System;

namespace Peritos.Common.Abstractions
{
    /// <summary>
    /// An exception that allows other layers to propagate exceptions. These will be caught and handled appropriately by the API layer.
    /// </summary>
    [Serializable]
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this exception.
        /// </summary>
        public int HttpStatusCode { get; }

        public ApiException(string message) : base(message)
        {
            HttpStatusCode = 400;
        }

        public ApiException(int statusCode, string message)
            : base(message)
        {
            HttpStatusCode = statusCode;
        }

        public ApiException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            HttpStatusCode = statusCode;
        }
    }
}
