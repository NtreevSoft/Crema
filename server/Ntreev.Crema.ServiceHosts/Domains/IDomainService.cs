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
using Ntreev.Crema.Data.Xml.Schema;

namespace Ntreev.Crema.ServiceHosts.Domains
{
    [ServiceContract(Namespace = CremaService.Namespace, SessionMode = SessionMode.Required, CallbackContract = typeof(IDomainEventCallback))]
    [ServiceKnownType(typeof(DBNull))]
    public interface IDomainService
    {
        [OperationContract]
        ResultBase<DomainContextMetaData> Subscribe(Guid authenticationToken);

        [OperationContract]
        ResultBase Unsubscribe();

        [OperationContract]
        ResultBase<DomainContextMetaData> GetMetaData();

        [OperationContract]
        ResultBase SetUserLocation(Guid domainID, DomainLocationInfo location);

        [OperationContract]
        ResultBase<DomainRowInfo[]> NewRow(Guid domainID, DomainRowInfo[] rows);

        [OperationContract]
        ResultBase RemoveRow(Guid domainID, DomainRowInfo[] rows);

        [OperationContract]
        ResultBase<DomainRowInfo[]> SetRow(Guid domainID, DomainRowInfo[] rows);

        [OperationContract]
        ResultBase SetProperty(Guid domainID, string propertyName, object value);

        [OperationContract]
        ResultBase BeginUserEdit(Guid domainID, DomainLocationInfo location);

        [OperationContract]
        ResultBase EndUserEdit(Guid domainID);

        [OperationContract]
        ResultBase<DomainUserInfo> Kick(Guid domainID, string userID, Guid token, string comment);

        [OperationContract]
        ResultBase SetOwner(Guid domainID, string userID, Guid token);

        [OperationContract]
        ResultBase DeleteDomain(Guid domainID, bool force);

        [OperationContract]
        bool IsAlive();
    }
}
