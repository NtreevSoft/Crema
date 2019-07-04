using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceHosts.Http.Extensions;

namespace Ntreev.Crema.ServiceHosts.Http
{
    public class ErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var faultException = new WebFaultException(HttpStatusCode.SeeOther);
            fault = (new
            {
                type = error.GetType().FullName,
                message = error.Message,
                stacktace = error.StackTrace
            }).ToJsonMessage();
        }

        public bool HandleError(Exception error)
        {
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorHandlerBehavior : Attribute, IEndpointBehavior, IServiceBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {

        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new ErrorHandler());
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = (ChannelDispatcher) channelDispatcherBase;
                channelDispatcher.ErrorHandlers.Add(new ErrorHandler());
            }
        }
    }
}
