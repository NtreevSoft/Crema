using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.SelfHost;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Ntreev.Crema.Services;
using Ntreev.Library.Extensions;

namespace Ntreev.Crema.ServiceHosts.Http
{
    [Export(typeof(IHttpServiceHost))]
    class HttpCommandServiceHost : IHttpServiceHost
    {
        private readonly ICremaHost cremaHost;
        private readonly CremaService cremaService;
        private HttpSelfHostServer server = null;

        [ImportingConstructor]
        public HttpCommandServiceHost(ICremaHost cremaHost, CremaService cremaService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
        }

        public void Open(int port)
        {
            var config = new HttpSelfHostConfiguration($"http://localhost:{port}");
            config.MapHttpAttributeRoutes();
            
            config.DependencyResolver = new MefDependencyResolver(this.cremaHost);
            config.Filters.Add(new CremaAuthorizeAttribute(this.cremaHost));
            config.Services.Replace(typeof(IExceptionHandler), new CremaExceptionHandler());
            
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter(true));

            this.server = new HttpSelfHostServer(config);
            this.server.OpenAsync().Wait();
        }

        public void Close()
        {
            this.server.CloseAsync().Wait();
        }
    }
}
