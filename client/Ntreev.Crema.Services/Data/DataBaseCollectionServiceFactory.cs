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

using Ntreev.Crema.Services.DataBaseCollectionService;
using Ntreev.Crema.Services.Users;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Threading;

namespace Ntreev.Crema.Services.Data
{
    class DataBaseCollectionServiceFactory : IDataBaseCollectionServiceCallback
    {
        private static readonly DataBaseCollectionServiceFactory empty = new DataBaseCollectionServiceFactory();

        private DataBaseCollectionServiceFactory()
        {

        }

        public static DataBaseCollectionServiceClient CreateServiceClient(string address, ServiceInfo serviceInfo, IDataBaseCollectionServiceCallback callback)
        {
            var binding = CremaHost.CreateBinding(serviceInfo);

            var endPointAddress = new EndpointAddress($"net.tcp://{address}:{serviceInfo.Port}/DataBaseCollectionService");
            var instanceContext = new InstanceContext(callback ?? empty);
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                instanceContext.SynchronizationContext = SynchronizationContext.Current;

            return new DataBaseCollectionServiceClient(instanceContext, binding, endPointAddress);
        }

        #region IDataBaseCollectionServiceCallback

        void IDataBaseCollectionServiceCallback.OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesCreated(SignatureDate signatureDate, string[] dataBaseNames, DataBaseInfo[] dataBaseInfos)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesDeleted(SignatureDate signatureDate, string[] dataBaseNames)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesRenamed(SignatureDate signatureDate, string[] dataBaseNames, string[] newDataBaseNames)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesLoaded(SignatureDate signatureDate, string[] dataBaseNames)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesUnloaded(SignatureDate signatureDate, string[] dataBaseNames)
        {
            throw new NotImplementedException();
        }

        public void OnDataBasesAuthenticationEntered(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo)
        {
            throw new NotImplementedException();
        }

        public void OnDataBasesAuthenticationLeft(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesInfoChanged(SignatureDate signatureDate, DataBaseInfo[] dataBaseInfos)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesStateChanged(SignatureDate signatureDate, string[] dataBaseNames, DataBaseState[] dataBaseStates)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes)
        {
            throw new NotImplementedException();
        }

        void IDataBaseCollectionServiceCallback.OnDataBasesLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}