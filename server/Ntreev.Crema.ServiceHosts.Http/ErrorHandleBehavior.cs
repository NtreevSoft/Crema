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
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceHosts.Http.Extensions;
using Ntreev.Crema.ServiceHosts.Http.Responses;

namespace Ntreev.Crema.ServiceHosts.Http
{
    public class ErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var response = new ErrorResponse
            {
                Type = error.GetType().FullName,
                Message = error.Message,
                StackTrace = error.StackTrace
            };
            fault = Message.CreateMessage(version, "", response, new DataContractJsonSerializer(typeof(ErrorResponse)));
            var wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

            var outgoingResponse = WebOperationContext.Current.OutgoingResponse;
            outgoingResponse.ContentType = "application/json";
            outgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
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
