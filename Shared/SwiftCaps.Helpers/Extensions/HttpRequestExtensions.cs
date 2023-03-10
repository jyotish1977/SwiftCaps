using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SwiftCaps.Helpers.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Performs HttpRequest body parsing and throws if parsing fails. 
        /// </summary>
        /// <typeparam name="T">Type to convert body to</typeparam>
        /// <param name="request">Instance of HttpRequest</param>
        /// <returns>Instance of T</returns>
        public static async Task<T> ParseBodyAndThrow<T>(this HttpRequest request)
        {
            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync().ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<T>(requestBody);
                if (data == null)
                {
                    throw new ArgumentNullException("Body", "Empty payload found.");
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new FormatException("Error parsing payload.",ex);
            }
        }
    }
}
