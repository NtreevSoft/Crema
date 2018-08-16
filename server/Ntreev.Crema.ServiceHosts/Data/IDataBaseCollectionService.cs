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
using Ntreev.Crema.Data;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Data
{
    [ServiceContract(Namespace = CremaService.Namespace, SessionMode = SessionMode.Required, CallbackContract = typeof(IDataBaseCollectionEventCallback))]
    public interface IDataBaseCollectionService
    {
        /// <summary>
        /// 특정 타입의 배열 형식이 메소드의 인자로 설정되지 않으면 반환값으로 사용될때 클라이언트의 코드에서 재사용되지 않고 임시 코드가 생성됨
        /// </summary>
        [OperationContract]
        ResultBase DefinitionType(LogInfo[] param1);

        [OperationContract]
        ResultBase<DataBaseCollectionMetaData> Subscribe(Guid authenticationToken);

        [OperationContract]
        ResultBase Unsubscribe();

        [OperationContract]
        ResultBase SetPublic(string dataBaseName);

        [OperationContract]
        ResultBase<AccessInfo> SetPrivate(string dataBaseName);

        [OperationContract]
        ResultBase<AccessMemberInfo> AddAccessMember(string dataBaseName, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase<AccessMemberInfo> SetAccessMember(string dataBaseName, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase RemoveAccessMember(string dataBaseName, string memberID);

        [OperationContract]
        ResultBase<LockInfo> Lock(string dataBaseName, string comment);

        [OperationContract]
        ResultBase Unlock(string dataBaseName);

        [OperationContract]
        ResultBase Load(string dataBaseName);

        [OperationContract]
        ResultBase Unload(string dataBaseName);

        [OperationContract]
        ResultBase<DataBaseInfo> Create(string dataBaseName, string comment);

        [OperationContract]
        ResultBase<DataBaseInfo> Copy(string dataBaseName, string newDataBaseName, string comment, bool force);

        [OperationContract]
        ResultBase Rename(string dataBaseName, string newDataBaseName);

        [OperationContract]
        ResultBase Delete(string dataBaseName);

        [OperationContract]
        ResultBase<LogInfo[]> GetLog(string dataBaseName, string revision);

        [OperationContract]
        ResultBase<DataBaseInfo> Revert(string dataBaseName, string revision);

        [OperationContract]
        ResultBase BeginTransaction(string dataBaseName);

        [OperationContract]
        ResultBase EndTransaction(string dataBaseName);

        [OperationContract]
        ResultBase CancelTransaction(string dataBaseName);

        [OperationContract]
        bool IsAlive();
    }
}