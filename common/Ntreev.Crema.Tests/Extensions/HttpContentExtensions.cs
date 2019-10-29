using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Tests.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> FromHttpContentAsync<T>(this HttpContent httpContent)
        {
            var json = await httpContent.ReadAsStringAsync();
            return json.FromJson<T>();
        }
    }
}
