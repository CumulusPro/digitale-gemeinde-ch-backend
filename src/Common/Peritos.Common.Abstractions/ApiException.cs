using System;

namespace Peritos.Common.Abstractions
{
    /// <summary>
    /// An exception that allows other layers to "bubble up" exceptions. These will be caught and handled appropriately. 
    /// </summary>
    [Serializable]
    public class ApiException : Exception
    {
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
