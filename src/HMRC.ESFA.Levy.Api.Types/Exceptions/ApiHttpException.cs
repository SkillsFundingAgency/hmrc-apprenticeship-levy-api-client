using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HMRC.ESFA.Levy.Api.Types.Exceptions
{
    public class ApiHttpException : HttpException
    {
        public int HttpCode => GetHttpCode();
        public string ResourceUri { get; set; }

        public ApiHttpException(int httpCode, string message, string resourceUri, Exception innerException = null) : base(httpCode, message, innerException)
        {
            ResourceUri = resourceUri;
        }
    }
}
