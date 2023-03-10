using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;
using Xamariners.Functions.Core.Helpers;
using Xamariners.RestClient.Helpers;

namespace Xamariners.Functions.Core.Extensions
{
    public static class HttpRequestDataExtensions
    {
        private static JsonSerializerOptions defaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        private static JsonObjectSerializer defaultSerializer = new JsonObjectSerializer(defaultSerializerOptions);
        private static HttpStatusCode MethodToStatusCode(string method, bool returnOkForPost = false) => method.ToLower() switch
        {
            "get" => HttpStatusCode.OK,
            "post" when returnOkForPost => HttpStatusCode.OK,
            "post" => HttpStatusCode.Created,
            "put" => HttpStatusCode.Accepted,
            "delete" => HttpStatusCode.NoContent,
            _ => throw new ArgumentOutOfRangeException(nameof(method), $"Invaid HTTP verb: {method}"),
        };

        private static HttpStatusCode ExceptionToStatusCode(Exception ex) => ex switch
        {
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            NotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.BadRequest,
        };


        public static bool EnsureAuthorizationHeader(this HttpRequestData request)
        {
            if(!request.Headers.Contains(HeaderNames.Authorization))throw new UnauthorizedAccessException("Access denied.");
            return true;
        }

        public static AuthenticationHeaderValue GetAuthenticationHeaderValue(this HttpRequestData request)
        {
            var authHeader = request.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault();
            var authHeaderValue = new AuthenticationHeaderValue("Bearer", authHeader.Replace("Bearer ", string.Empty));
            return authHeaderValue;
        }

        public static string ReadHttpRequestData(this HttpRequestData request)
        {
            return MemoryStreamHelpers.ReadToEnd(request.Body);
        }

        public static async Task<T> DeserializePayloadAsync<T>(this HttpRequestData request)
        {
            try
            {
                var instance = await JsonSerializer.DeserializeAsync<T>(request.Body, defaultSerializerOptions);
                return instance;
            }
            catch
            {
                throw new ArgumentException("Missing payload or invalid payload provided.");
            }
        }

        public static async Task<HttpResponseData> CreateSuccessResponseAsync<T>(this HttpRequestData req, T data, bool returnOkForPost = false)
        {
            var statusCode = MethodToStatusCode(req.Method, returnOkForPost);
            var response = req.CreateResponse();
            var payload = ServiceResponseHelpers.SuccessServiceResponse(data, "Ok");
            payload.StatusCode = (int)statusCode;
            await response.WriteAsJsonAsync(payload, defaultSerializer);
            response.StatusCode = statusCode;
            return response;
        }

        public static async Task<HttpResponseData> CreateErrorResponseAsync<T>(this HttpRequestData req, Exception ex)
        {
            var statusCode = ExceptionToStatusCode(ex);
            var data = ServiceResponseHelpers.ErrorServiceResponse<T>("Error processing request.", new string[] { ex.Message });
            data.StatusCode = (int)statusCode;
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(data, defaultSerializer);
            response.StatusCode = statusCode;
            return response;
        }
    }
}
