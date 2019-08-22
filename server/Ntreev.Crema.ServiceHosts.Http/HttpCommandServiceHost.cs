using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Ntreev.Crema.ServiceHosts.Http.Apis;
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

        [ImportingConstructor]
        public HttpCommandServiceHost(ICremaHost cremaHost, CremaService cremaService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
        }

        public void Open(int port)
        {
            this.server = WebApp.Start($"http://*:{port}", app =>
            {
                var listener = (OwinHttpListener)app.Properties[typeof(OwinHttpListener).FullName];
                listener.GetRequestProcessingLimits(out var maxAccepts, out var maxRequests);
                listener.SetRequestProcessingLimits(maxAccepts, 1);

                var config = new HttpConfiguration();
                config.ConfigureCrema(this.cremaHost)
                    .ConfigureCremaSwagger();
                app.UseWebApi(config);
            });
        }

        public void Close()
        {
            this.server.Dispose();
        }
    }
}
