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

namespace Ntreev.Crema.ServiceHosts.Users
{
    public interface IUserEventCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo);

        [OperationContract(IsOneWay = true)]
        void OnUsersChanged(SignatureDate signatureDate, UserInfo[] userInfos);

        [OperationContract(IsOneWay = true)]
        void OnUsersStateChanged(SignatureDate signatureDate, string[] userIDs, UserState[] states);

        [OperationContract(IsOneWay = true)]
        void OnUserItemsCreated(SignatureDate signatureDate, string[] itemPaths, UserInfo?[] args);

        [OperationContract(IsOneWay = true)]
        void OnUserItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames);

        [OperationContract(IsOneWay = true)]
        void OnUserItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths);

        [OperationContract(IsOneWay = true)]
        void OnUserItemsDeleted(SignatureDate signatureDate, string[] itemPaths);

        [OperationContract(IsOneWay = true)]
        void OnUsersLoggedIn(SignatureDate signatureDate, string[] userIDs);

        [OperationContract(IsOneWay = true)]
        void OnUsersLoggedOut(SignatureDate signatureDate, string[] userIDs);

        [OperationContract(IsOneWay = true)]
        void OnUsersKicked(SignatureDate signatureDate, string[] userIDs, string[] comments);

        [OperationContract(IsOneWay = true)]
        void OnUsersBanChanged(SignatureDate signatureDate, BanInfo[] banInfos, BanChangeType changeType, string[] comments);

        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(SignatureDate signatureDate, string[] userIDs, string message, MessageType messageType);

        [OperationContract]
        bool OnPing();
    }
}
