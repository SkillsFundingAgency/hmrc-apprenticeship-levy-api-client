using System;

namespace HMRC.ESFA.Levy.Api.Types.Exceptions
{
    using System.Net;
    using System.Net.Http;

    public class ApiHttpException : Exception
    {
    public int HttpCode { get; set; }
    public string ResourceUri { get; set; }

        public string ResponseBody { get; set; }

        public ApiHttpException(int httpCode, string message, string resourceUri, string body, Exception innerException = null) : base(message, innerException)
        {
            HttpCode = httpCode;
            ResponseBody = body;
            ResourceUri = resourceUri;
        }
    }
}
