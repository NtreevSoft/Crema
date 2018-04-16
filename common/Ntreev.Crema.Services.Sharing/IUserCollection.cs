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
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ntreev.Crema.Services
{
    public interface IUserCollection : IReadOnlyCollection<IUser>, IEnumerable<IUser>, INotifyCollectionChanged, IServiceProvider, IDispatcherObject
    {
        bool Contains(string userID);

        IUser this[string userID] { get; }

        event ItemsCreatedEventHandler<IUser> UsersCreated;

        event ItemsMovedEventHandler<IUser> UsersMoved;

        event ItemsRenamedEventHandler<IUser> UsersRenamed;

        event ItemsDeletedEventHandler<IUser> UsersDeleted;

        event ItemsEventHandler<IUser> UsersStateChanged;

        event ItemsEventHandler<IUser> UsersChanged;

        event ItemsEventHandler<IUser> UsersLoggedIn;

        event ItemsEventHandler<IUser> UsersLoggedOut;

        event ItemsEventHandler<IUser> UsersKicked;

        event ItemsEventHandler<IUser> UsersBanChanged;

        event EventHandler<MessageEventArgs> MessageReceived;
    }
}
