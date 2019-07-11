using System.ComponentModel.Composition;
using System.Web.Http.SelfHost;
using Ntreev.Crema.ServiceHosts.Http.Apis;
using Ntreev.Crema.Services;

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

            config.ConfigureCrema(this.cremaHost)
                .ConfigureCremaSwagger();

            this.server = new HttpSelfHostServer(config);
            this.server.OpenAsync().Wait();
        }

        public void Close()
        {
            this.server.CloseAsync().Wait();
        }
    }
}
