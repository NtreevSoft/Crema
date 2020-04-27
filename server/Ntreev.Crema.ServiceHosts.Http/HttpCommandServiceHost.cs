using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Ntreev.Crema.ServiceHosts.Http.Apis;
using Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.GQL.Playground;
using Ntreev.Crema.Services;
using Owin;

namespace Ntreev.Crema.ServiceHosts.Http
{
    [Export(typeof(IHttpServiceHost))]
    class HttpCommandServiceHost : IHttpServiceHost
    {
        private readonly ICremaHost cremaHost;
        private readonly CremaService cremaService;
        private IDisposable server;
        
        private static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

        [ImportingConstructor]
        public HttpCommandServiceHost(ICremaHost cremaHost, CremaService cremaService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
        }

        public void Open(int port)
        {
            var option = new StartOptions
            {
                Urls = { $"http://*:{port}" },
                ServerFactory = IsMono ? typeof(OwinServerFactory).Namespace : null
            };
            this.server = WebApp.Start(option, app =>
            {
                var config = new HttpConfiguration();
                config.ConfigureCrema(this.cremaHost)
                    .ConfigureCremaSwagger();
                app.UseWebApi(config);
                app.Use<PlaygroundMiddleware>(new GraphQLPlaygroundOptions());
            });
        }

        public void Close()
        {
            this.server.Dispose();
        }
    }
}
