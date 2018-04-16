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

using Ntreev.Crema.Data;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Ntreev.Crema.Services
{
    public interface IDomain : IServiceProvider, IDispatcherObject, IExtendedProperties
    {
        void Delete(Authentication authentication, bool isCancel);

        void BeginUserEdit(Authentication authentication, DomainLocationInfo location);

        void EndUserEdit(Authentication authentication);

        DomainRowInfo[] NewRow(Authentication authentication, DomainRowInfo[] rows);

        DomainRowInfo[] SetRow(Authentication authentication, DomainRowInfo[] rows);

        void RemoveRow(Authentication authentication, DomainRowInfo[] rows);

        void SetProperty(Authentication authentication, string propertyName, object value);

        void SetUserLocation(Authentication authentication, DomainLocationInfo location);

        DomainUserInfo Kick(Authentication authentication, string userID, string comment);

        void SetOwner(Authentication authentication, string userID);

        Guid ID { get; }

        Guid DataBaseID { get; }

        object Source { get; }

        object Host { get; }

        DomainInfo DomainInfo { get; }

        DomainState DomainState { get; }

        IDomainUserCollection Users { get; }

        event EventHandler<DomainUserEventArgs> UserAdded;

        event EventHandler<DomainUserEventArgs> UserChanged;

        event EventHandler<DomainUserRemovedEventArgs> UserRemoved;

        event EventHandler<DomainRowEventArgs> RowAdded;

        event EventHandler<DomainRowEventArgs> RowRemoved;

        event EventHandler<DomainRowEventArgs> RowChanged;

        event EventHandler<DomainPropertyEventArgs> PropertyChanged;

        event EventHandler DomainInfoChanged;

        event EventHandler DomainStateChanged;

        event EventHandler<DomainDeletedEventArgs> Deleted;

        DomainMetaData GetMetaData(Authentication authentication);
    }
}
