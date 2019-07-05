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
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Ntreev.Crema.ServiceHosts.Http.Requests.Commands;

namespace Ntreev.Crema.ServiceHosts.Http.Commands
{
    [ServiceContract]
    public interface IHttpCommandsDataBaseService
    {
        [OperationContract]
        [WebGet(UriTemplate = "databases", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message GetDataBaseList();

        [OperationContract]
        [WebGet(UriTemplate = "databases/{databaseName}/info?tags={tags}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message GetDataBaseInfo(string databaseName, string tags);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/log", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message GetDataBaseLogInfo(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/enter", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void EnterDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/leave", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LeaveDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/load", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void LoadDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/unload", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void UnloadDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/entered", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message IsDataBaseEntered(string databaseName);

        [OperationContract]
        [WebGet(UriTemplate = "databases/{databaseName}/loaded", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message IsDataBaseLoaded(string databaseName);

        [OperationContract]
        [WebGet(UriTemplate = "databases/{databaseName}/contains", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Message ContainsDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/copy", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void CopyDataBase(string databaseName, CopyDataBaseRequest request);

        [OperationContract]
        [WebGet(UriTemplate = "databases/{databaseName}/delete", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void DeleteDataBase(string databaseName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/create", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void CreateDataBase(string databaseName, CreateDataBaseRequest request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "databases/{databaseName}/rename", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void RenameDataBase(string databaseName, RenameDataBaseRequest request);
    }
}