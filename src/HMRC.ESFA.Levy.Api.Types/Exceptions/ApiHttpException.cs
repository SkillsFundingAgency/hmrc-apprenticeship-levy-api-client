using System;
using System.Web;

namespace HMRC.ESFA.Levy.Api.Types.Exceptions
{
    public class ApiHttpException : HttpException
    {
        public int HttpCode => GetHttpCode();
        public string ResourceUri { get; set; }

        public string ResponseBody { get; set; }

        public ApiHttpException(int httpCode, string message, string resourceUri, string body, Exception innerException = null) : base(httpCode, message, innerException)
        {
            ResponseBody = body;
            ResourceUri = resourceUri;
        }
    }
}
