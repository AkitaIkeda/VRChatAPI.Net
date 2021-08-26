using System;
using System.Net.Http;

namespace VRChatAPI.Exceptions
{
    /// <summary>
    /// No valid auth header or ApiKey is set
    /// </summary>
    /// <remarks>
    /// Sent request was not authorized and received 401 response
    /// To verify the auth, call <see cref="Endpoints.SystemAPI.VerifyAuth">SystemApi.VerifyAuth</see>
    /// </remarks>
    /// <seealso cref="Endpoints.SystemAPI.VerifyAuth">SystemApi.VerifyAuth</seealso>
    [Serializable]
    internal class UnauthorizedRequestException : HttpRequestException
    {
        public UnauthorizedRequestException()
        {
        }
        public UnauthorizedRequestException(string message) : base(message)
        {
        }
        public UnauthorizedRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}