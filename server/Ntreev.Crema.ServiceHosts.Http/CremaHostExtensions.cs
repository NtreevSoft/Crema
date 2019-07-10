//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Ntreev.Crema.Services;
using Swashbuckle.Application;

namespace Ntreev.Crema.ServiceHosts.Http
{
    static class CremaHostExtensions
    {
        public static HttpConfiguration ConfigureCrema(this HttpConfiguration config, ICremaHost cremaHost)
        {
            config.MapHttpAttributeRoutes();
            config.DependencyResolver = new MefDependencyResolver(cremaHost);
            config.Filters.Add(new CremaAuthorizeAttribute(cremaHost));
            config.Services.Replace(typeof(IExceptionHandler), new CremaExceptionHandler());

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter(true));

            return config;
        }

        public static HttpConfiguration ConfigureCremaSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(o =>
                {
                    o.MultipleApiVersions(
                        ResolveVersionSupportByRouteConstraint,
                        builder =>
                        {
                            builder.Version("v1", "Crema commands api v1");
                        });
                })
                .EnableSwaggerUi(o =>
                {
                    o.EnableDiscoveryUrlSelector();
                    o.InjectJavaScript(Assembly.GetExecutingAssembly(),
                        Assembly.GetExecutingAssembly().GetName().Name + ".SwaggerAuthorization.js");
                });

            return config;
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {
            return apiDesc.RelativePath.StartsWith($"api/{targetApiVersion}/");
        }
    }
}
