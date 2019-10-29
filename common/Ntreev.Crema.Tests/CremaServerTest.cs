using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services;
using Xunit;
using Xunit.Abstractions;

namespace Ntreev.Crema.Tests
{
    public class CremaServerTest
    {
        public CremaServerTest()
        {
        }

        public HttpClient GetHttpClient(Authentication authentication = null, IReadOnlyDictionary<string, string> headers = null)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:4104"),
                Timeout = TimeSpan.FromSeconds(5),
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (authentication != null)
            {
                httpClient.DefaultRequestHeaders.Add("Token", authentication.Token.ToString());
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            return httpClient;
        }
    }
}
