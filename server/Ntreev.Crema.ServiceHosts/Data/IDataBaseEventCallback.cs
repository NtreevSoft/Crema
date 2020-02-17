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

using System.ServiceModel;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceHosts.Data
{
    public interface IDataBaseEventCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo);

        [OperationContract(IsOneWay = true)]
        void OnTablesChanged(SignatureDate signatureDate, TableInfo[] tableInfos);

        [OperationContract(IsOneWay = true)]
        void OnTablesDetailInfoChanged(SignatureDate signatureDate, TableDetailInfo[] tableDetailInfos);

        [OperationContract(IsOneWay = true)]
        void OnTablesStateChanged(SignatureDate signatureDate, string[] tableNames, TableState[] states);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsCreated(SignatureDate signatureDate, string[] itemPaths, TableInfo?[] args);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsDeleted(SignatureDate signatureDate, string[] itemPaths);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes);

        [OperationContract(IsOneWay = true)]
        void OnTableItemsLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments);

        [OperationContract(IsOneWay = true)]
        void OnTypesChanged(SignatureDate signatureDate, TypeInfo[] typeInfos);

        [OperationContract(IsOneWay = true)]
        void OnTypesStateChanged(SignatureDate signatureDate, string[] typeNames, TypeState[] states);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsCreated(SignatureDate signatureDate, string[] itemPaths, TypeInfo?[] args);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsRenamed(SignatureDate signatureDate, string[] itemPaths, string[] newNames);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsMoved(SignatureDate signatureDate, string[] itemPaths, string[] parentPaths);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsDeleted(SignatureDate signatureDate, string[] itemPaths);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsAccessChanged(SignatureDate signatureDate, AccessChangeType changeType, AccessInfo[] accessInfos, string[] memberIDs, AccessType[] accessTypes);

        [OperationContract(IsOneWay = true)]
        void OnTypeItemsLockChanged(SignatureDate signatureDate, LockChangeType changeType, LockInfo[] lockInfos, string[] comments);
    }
}
