using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ntreev.Crema.Tests.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {

        };

        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings ?? DefaultSettings);
        }

        public static T FromJson<T>(this string obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(obj, settings ?? DefaultSettings);
        }
    }
}
