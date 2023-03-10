using Microsoft.Azure.Functions.Worker.Http;
using Xamariners.Functions.Core.Helpers;

namespace Xamariners.Functions.Core.Extensions
{
    public static class HttpResponseDataExtensions
    {
        public static string ReadHttpResponseData(this HttpResponseData response)
        {
            return MemoryStreamHelpers.ReadToEnd(response.Body);
        }
    }
}
