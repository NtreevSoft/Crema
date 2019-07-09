using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http
{
    public class MefDependencyResolver : IDependencyResolver
    {
        private readonly ICremaHost cremaHost;

        public MefDependencyResolver(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return this.cremaHost.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return this.cremaHost.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
        }

        public void Dispose()
        {
        }
    }
}
