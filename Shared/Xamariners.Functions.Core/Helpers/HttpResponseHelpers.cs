using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Xamariners.RestClient.Helpers.Models;

namespace Xamariners.Functions.Core.Helpers
{
    public static class HttpResponseHelpers
    {
        public static HttpResponseMessage CreateHttpServiceResponseMessage<T>(this ServiceResponse<T> response)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(response))
            };

            return msg;
        }

        public static HttpResponseMessage CreateHttpContentResponseMessage(this HttpContent content)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            return msg;
        }
    }
}
