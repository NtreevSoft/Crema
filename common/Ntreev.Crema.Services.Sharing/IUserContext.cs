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
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Security;

namespace Ntreev.Crema.Services
{
    public interface IUserContext : IEnumerable<IUserItem>, IServiceProvider, IDispatcherObject
    {
        UserContextMetaData GetMetaData(Authentication authentication);

        void NotifyMessage(Authentication authentication, string[] userIDs, string message);

        bool Contains(string itemPath);

        IUserCollection Users { get; }

        IUserCategoryCollection Categories { get; }

        IUserCategory Root { get; }

        IUserItem this[string itemPath] { get; }

        event ItemsCreatedEventHandler<IUserItem> ItemsCreated;

        event ItemsRenamedEventHandler<IUserItem> ItemsRenamed;

        event ItemsMovedEventHandler<IUserItem> ItemsMoved;

        event ItemsDeletedEventHandler<IUserItem> ItemsDeleted;

        event ItemsEventHandler<IUserItem> ItemsChanged;

        event ItemsEventHandler<AuthenticationInfo> UsersLoggedIn;

        event ItemsEventHandler<AuthenticationInfo> UsersLoggedOut;

        event ItemsEventHandler<IUser> UsersKicked;

        event ItemsEventHandler<IUserAuthentication> UserAuthenticationsKicked;

        event ItemsEventHandler<IUser> UsersBanChanged;

        event EventHandler<MessageEventArgs> MessageReceived;

#if SERVER
        Authentication Login(string userID, SecureString password);

        void Logout(Authentication authentication);

        Authentication Authenticate(Guid authenticationToken);

        bool IsAuthenticated(string userID, Guid token);

        bool IsOnlineUser(string userID, SecureString password);
#endif
    }

    public static class IUserContextExtensions
    {
        public static void NotifyMessage(this IUserContext userContext, Authentication authentication, string message)
        {
            userContext.NotifyMessage(authentication, new string[] { }, message);
        }
    }
}
