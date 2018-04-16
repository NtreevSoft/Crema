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

using Ntreev.Crema.RuntimeService.DescriptorService;
using Ntreev.Crema.RuntimeService.ServiceClient;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Ntreev.Crema.RuntimeService
{
    class RuntimeServiceFactory
    {
        public static RuntimeServiceClient CreateServiceClient(string address)
        {
            var binding = CreateBinding(TimeSpan.MaxValue);
            var ip = AddressUtility.GetIPAddress(address);
            var port = GetServicePort(address);
            var endPointAddress = new EndpointAddress($"net.tcp://{ip}:{port}/RuntimeService");

            return new RuntimeServiceClient(binding, endPointAddress);
        }

        private static int GetServicePort(string address)
        {
            var binding = CreateBinding(TimeSpan.MaxValue);
            var endPointAddress = new EndpointAddress($"net.tcp://{AddressUtility.ConnectionAddress(address)}/DescriptorService");
            var serviceClient = new DescriptorServiceClient(binding, endPointAddress);

            try
            {
                var serviceInfos = serviceClient.GetServiceInfos();

                foreach (var item in serviceInfos)
                {
                    if (item.Name == nameof(RuntimeService))
                    {
                        return item.Port;
                    }
                }

                throw new InvalidOperationException();
            }
            finally
            {
                serviceClient.Close();
            }
        }

        private static Binding CreateBinding(TimeSpan timeSpan)
        {
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            //binding.SendTimeout = timeSpan;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            //binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas = XmlDictionaryReaderQuotas.Max;

            return binding;
        }
    }
}
