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
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;

namespace Ntreev.Crema.Services
{
    public interface ITypeContext : IEnumerable<ITypeItem>, IServiceProvider
    {
        bool Contains(string itemPath);

        ITypeCollection Types { get; }

        ITypeCategoryCollection Categories { get; }

        ITypeCategory Root { get; }

        ITypeItem this[string itemPath] { get; }

        event ItemsCreatedEventHandler<ITypeItem> ItemsCreated;

        event ItemsRenamedEventHandler<ITypeItem> ItemsRenamed;

        event ItemsMovedEventHandler<ITypeItem> ItemsMoved;

        event ItemsDeletedEventHandler<ITypeItem> ItemsDeleted;

        event ItemsEventHandler<ITypeItem> ItemsChanged;

        event ItemsEventHandler<ITypeItem> ItemsAccessChanged;

        event ItemsEventHandler<ITypeItem> ItemsLockChanged;
    }
}
