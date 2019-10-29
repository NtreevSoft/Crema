using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Tests.Extensions
{
    public static class HttpResponseExtensions
    {
        public static Task<T> FromHttpMessageResponse<T>(this HttpResponseMessage message)
        {
            message.EnsureSuccessStatusCode();
            return message.Content.FromHttpContentAsync<T>();
        }
    }
}
