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
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Ntreev.Crema.Services
{
    public interface IDomainCollection : IReadOnlyCollection<IDomain>, IEnumerable<IDomain>, INotifyCollectionChanged, IServiceProvider, IDispatcherObject
    {
        bool Contains(Guid domainID);

        IDomain this[Guid domainID] { get; }

        DomainMetaData[] GetMetaData(Authentication authentication);

        event EventHandler<DomainEventArgs> DomainCreated;

        event EventHandler<DomainDeletedEventArgs> DomainDeleted;

        event EventHandler<DomainEventArgs> DomainInfoChanged;

        event EventHandler<DomainEventArgs> DomainStateChanged;

        event EventHandler<DomainUserEventArgs> DomainUserAdded;

        event EventHandler<DomainUserEventArgs> DomainUserChanged;

        event EventHandler<DomainUserRemovedEventArgs> DomainUserRemoved;

        event EventHandler<DomainRowEventArgs> DomainRowAdded;

        event EventHandler<DomainRowEventArgs> DomainRowChanged;

        event EventHandler<DomainRowEventArgs> DomainRowRemoved;

        event EventHandler<DomainPropertyEventArgs> DomainPropertyChanged;
    }
}
