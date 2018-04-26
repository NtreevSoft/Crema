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
using System.Linq;
using System.Text;
using System.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Data
{
    public interface IDataBaseCollectionEventCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesCreated(SignatureDate signatureDate, string[] dataBaseNames, DataBaseInfo[] dataBaseInfos, string comment);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesRenamed(SignatureDate signatureDate, string[] dataBaseNames, string[] newDataBaseNames);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesDeleted(SignatureDate signatureDate, string[] dataBaseNames);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesLoaded(SignatureDate signatureDate, string[] dataBaseNames);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesUnloaded(SignatureDate signatureDate, string[] dataBaseNames);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesResetting(SignatureDate signatureDate, string[] dataBaseNames);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesReset(SignatureDate signatureDate, string[] dataBaseNames, DomainMetaData[] metaDatas);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesAuthenticationEntered(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesAuthenticationLeft(SignatureDate signatureDate, string[] dataBaseNames, AuthenticationInfo authenticationInfo);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesInfoChanged(SignatureDate signatureDate, DataBaseInfo[] dataBaseInfos);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesStateChanged(SignatureDate signatureDate, string[] dataBaseNames, DataBaseState[] dataBaseStates);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes);

        [OperationContract(IsOneWay = true)]
        void OnDataBasesLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments);
    }
}
