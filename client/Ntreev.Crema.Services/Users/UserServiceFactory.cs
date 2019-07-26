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

using Ntreev.Crema.Services.UserService;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Threading;

namespace Ntreev.Crema.Services.Users
{
    class UserServiceFactory : IUserServiceCallback
    {
        private readonly EndpointAddress endPointAddress;
        private readonly Binding binding;
        private readonly InstanceContext instanceContext;

        private UserServiceFactory(string address, ServiceInfo serviceInfo, IUserServiceCallback userServiceCallback)
        {
            this.binding = CremaHost.CreateBinding(serviceInfo);
            this.endPointAddress = new EndpointAddress($"net.tcp://{address}:{serviceInfo.Port}/UserService");
            this.instanceContext = new InstanceContext(userServiceCallback ?? (this));
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                this.instanceContext.SynchronizationContext = SynchronizationContext.Current;
        }

        public static UserServiceClient CreateServiceClient(string address, ServiceInfo serviceInfo, IUserServiceCallback userServiceCallback)
        {
            var factory = new UserServiceFactory(address, serviceInfo, userServiceCallback);
            return new UserServiceClient(factory.instanceContext, factory.binding, factory.endPointAddress);
        }

        #region IUserServiceCallback

        void IUserServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersChanged(SignatureDate signatureDate, UserInfo[] userInfos)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersStateChanged(SignatureDate signatureDate, string[] userIDs, UserState[] states)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUserItemsCreated(SignatureDate signatureDate, string[] itemPaths, UserInfo?[] args)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUserItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUserItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUserItemsDeleted(SignatureDate signatureDate, string[] itemPaths)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersLoggedIn(SignatureDate signatureDate, AuthenticationInfo[] authenticationInfos)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersLoggedOut(SignatureDate signatureDate, AuthenticationInfo[] authenticationInfos)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersKicked(SignatureDate signatureDate, string[] userIDs, string[] comments)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnUsersBanChanged(SignatureDate signatureDate, BanInfo[] banInfos, BanChangeType changeType, string[] comments)
        {
            throw new NotImplementedException();
        }

        void IUserServiceCallback.OnMessageReceived(SignatureDate signatureDate, string[] userIDs, string message, MessageType messageType)
        {
            throw new NotImplementedException();
        }

        bool IUserServiceCallback.OnPing()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
