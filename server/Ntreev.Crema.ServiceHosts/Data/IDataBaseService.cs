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
using System.ServiceModel;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceHosts.Data
{
    [ServiceContract(Namespace = CremaService.Namespace, SessionMode = SessionMode.Required, CallbackContract = typeof(IDataBaseEventCallback))]
    public interface IDataBaseService
    {
        /// <summary>
        /// 특정 타입의 배열 형식이 메소드의 인자로 설정되지 않으면 반환값으로 사용될때 클라이언트의 코드에서 재사용되지 않고 임시 코드가 생성됨
        /// </summary>
        [OperationContract]
        ResultBase DefinitionType(LogInfo[] param1, FindResultInfo[] param2);

        [OperationContract]
        ResultBase<DataBaseMetaData> Subscribe(Guid authenticationToken, string dataBaseName);

        [OperationContract]
        ResultBase<DataBaseMetaData> Subscribe2(Guid authenticationToken, string dataBaseName, string version);

        [OperationContract]
        ResultBase Unsubscribe();

        [OperationContract]
        ResultBase<DataBaseMetaData> GetMetaData();

        [OperationContract]
        ResultBase<CremaDataSet> GetDataSet(long revision);

        [OperationContract]
        ResultBase NewTableCategory(string categoryPath);

        [OperationContract]
        ResultBase<CremaDataSet> GetTableItemDataSet(string itemPath, long revision);

        [OperationContract]
        ResultBase ImportTables(CremaDataSet dataSet, string comment);

        [OperationContract]
        ResultBase RenameTableItem(string itemPath, string newName);

        [OperationContract]
        ResultBase MoveTableItem(string itemPath, string parentPath);

        [OperationContract]
        ResultBase DeleteTableItem(string itemPath);

        [OperationContract]
        ResultBase SetPublicTableItem(string itemPath);

        [OperationContract]
        ResultBase<AccessInfo> SetPrivateTableItem(string itemPath);

        [OperationContract]
        ResultBase<AccessMemberInfo> AddAccessMemberTableItem(string itemPath, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase<AccessMemberInfo> SetAccessMemberTableItem(string itemPath, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase RemoveAccessMemberTableItem(string itemPath, string memberID);

        [OperationContract]
        ResultBase<LockInfo> LockTableItem(string itemPath, string comment);

        [OperationContract]
        ResultBase UnlockTableItem(string itemPath);

        [OperationContract]
        ResultBase SetTableItemProperty(string itemPath, string propertyName, string value);

        [OperationContract]
        ResultBase<LogInfo[]> GetTableItemLog(string itemPath);

        [OperationContract]
        ResultBase<long> GetTableRevision(string itemPath);

        [OperationContract]
        ResultBase<TableDetailInfo> GetTableDetailInfo(string tableName);

        [OperationContract]
        ResultBase<FindResultInfo[]> FindTableItem(string itemPath, string text, FindOptions options);

        [OperationContract]
        ResultBase<TableInfo[]> CopyTable(string tableName, string newTableName, string categoryPath, bool copyXml);

        [OperationContract]
        ResultBase<TableInfo[]> InheritTable(string tableName, string newTableName, string categoryPath, bool copyXml);

        [OperationContract]
        ResultBase<DomainMetaData> EnterTableContentEdit(string tableName);

        [OperationContract]
        ResultBase<DomainMetaData> LeaveTableContentEdit(string tableName);

        [OperationContract]
        ResultBase<DomainMetaData> BeginTableContentEdit(string tableName);

        [OperationContract]
        ResultBase<TableInfo[]> EndTableContentEdit(string tableName);

        [OperationContract]
        ResultBase CancelTableContentEdit(string tableName);

        [OperationContract]
        ResultBase<DomainMetaData> BeginTableTemplateEdit(string tableName);

        [OperationContract]
        ResultBase<DomainMetaData> BeginNewTable(string itemPath);

        [OperationContract(IsInitiating = true)]
        ResultBase<TableInfo> EndTableTemplateEdit(Guid domainID);

        [OperationContract]
        ResultBase CancelTableTemplateEdit(Guid domainID);

        [OperationContract]
        ResultBase NewTypeCategory(string categoryPath);

        [OperationContract]
        ResultBase<CremaDataSet> GetTypeItemDataSet(string itemPath, long revision);

        [OperationContract]
        ResultBase ImportTypes(CremaDataSet dataSet, string comment);

        [OperationContract]
        ResultBase RenameTypeItem(string itemPath, string newName);

        [OperationContract]
        ResultBase MoveTypeItem(string itemPath, string parentPath);

        [OperationContract]
        ResultBase DeleteTypeItem(string itemPath);

        [OperationContract]
        ResultBase<TypeInfo> CopyType(string typeName, string newTypeName, string categoryPath);

        [OperationContract]
        ResultBase<DomainMetaData> BeginTypeTemplateEdit(string typeName);

        [OperationContract]
        ResultBase<DomainMetaData> BeginNewType(string categoryPath);

        [OperationContract]
        ResultBase<TypeInfo> EndTypeTemplateEdit(Guid domainID);

        [OperationContract]
        ResultBase CancelTypeTemplateEdit(Guid domainID);

        [OperationContract]
        ResultBase SetPublicTypeItem(string itemPath);

        [OperationContract]
        ResultBase<AccessInfo> SetPrivateTypeItem(string itemPath);

        [OperationContract]
        ResultBase<AccessMemberInfo> AddAccessMemberTypeItem(string itemPath, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase<AccessMemberInfo> SetAccessMemberTypeItem(string itemPath, string memberID, AccessType accessType);

        [OperationContract]
        ResultBase RemoveAccessMemberTypeItem(string itemPath, string memberID);

        [OperationContract]
        ResultBase<LockInfo> LockTypeItem(string itemPath, string comment);

        [OperationContract]
        ResultBase UnlockTypeItem(string itemPath);

        [OperationContract]
        ResultBase SetTypeItemProperty(string itemPath, string propertyName, string value);

        [OperationContract]
        ResultBase<LogInfo[]> GetTypeItemLog(string itemPath);

        [OperationContract]
        ResultBase<long> GetTypeRevision(string itemPath);

        [OperationContract]
        ResultBase<FindResultInfo[]> FindTypeItem(string itemPath, string text, FindOptions options);

        [OperationContract]
        bool IsAlive();
    }
}