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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Jint.Native;

namespace Ntreev.Crema.Javascript.Methods
{
    //[Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class AddEventListenerMethod : ScriptMethodBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public AddEventListenerMethod(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        protected override Delegate CreateDelegate()
        {
            return new Action<CremaEvents, Action<IDictionary<string, object>>>(this.AddEventListener);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        private void AddEventListener(CremaEvents eventName, Action<IDictionary<string, object>> action)
        {
            switch (eventName)
            {
                case CremaEvents.UserStateChanged:
                case CremaEvents.UserChanged:
                case CremaEvents.UserItemCreated:
                case CremaEvents.UserItemRenamed:
                case CremaEvents.UserItemMoved:
                case CremaEvents.UserItemDeleted:
                case CremaEvents.UserLoggedIn:
                case CremaEvents.UserLoggedOut:
                case CremaEvents.UserKicked:
                case CremaEvents.UserBanChanged:
                case CremaEvents.MessageReceived:
                    this.RegisterUserEventHandler(eventName, action);
                    return;

                case CremaEvents.DomainCreated:
                    return;
                case CremaEvents.DomainDeleted:
                    return;
                case CremaEvents.DomainInfoChanged:
                    return;
                case CremaEvents.DomainStateChanged:
                    return;
                case CremaEvents.DomainUserAdded:
                    return;
                case CremaEvents.DomainUserRemoved:
                    return;
                case CremaEvents.DomainUserChanged:
                    return;
                case CremaEvents.DomainRowAdded:
                    return;
                case CremaEvents.DomainRowChanged:
                    return;
                case CremaEvents.DomainRowRemoved:
                    return;
                case CremaEvents.DomainPropertyChanged:
                    return;

                case CremaEvents.DataBaseCreated:
                    return;
                case CremaEvents.DataBaseRenamed:
                    return;
                case CremaEvents.DataBaseDeleted:
                    return;
                case CremaEvents.DataBaseLoaded:
                    return;
                case CremaEvents.DataBaseUnloaded:
                    return;
                case CremaEvents.DataBaseResetting:
                    return;
                case CremaEvents.DataBaseReset:
                    return;
                case CremaEvents.DataBaseAuthenticationEntered:
                    return;
                case CremaEvents.DataBaseAuthenticationLeft:
                    return;
                case CremaEvents.DataBaseInfoChanged:
                    return;
                case CremaEvents.DataBaseStateChanged:
                    return;
                case CremaEvents.DataBaseAccessChanged:
                    return;
                case CremaEvents.DataBaseLockChanged:
                    return;
            }

            throw new NotImplementedException();
        }

        private void RegisterUserEventHandler(CremaEvents eventName, Action<IDictionary<string, object>> action)
        {
            if (this.cremaHost.GetService(typeof(IUserContext)) is IUserContext userContext)
            {
                userContext.Dispatcher.Invoke(() =>
                {
                    switch (eventName)
                    {
                        case CremaEvents.UserStateChanged:
                            userContext.Users.UsersStateChanged += Users_UsersStateChanged;
                            break;
                        case CremaEvents.UserChanged:
                            break;
                        case CremaEvents.UserItemCreated:
                            break;
                        case CremaEvents.UserItemRenamed:
                            break;
                        case CremaEvents.UserItemMoved:
                            break;
                        case CremaEvents.UserItemDeleted:
                            break;
                        case CremaEvents.UserLoggedIn:
                            break;
                        case CremaEvents.UserLoggedOut:
                            break;
                        case CremaEvents.UserKicked:
                            break;
                        case CremaEvents.UserBanChanged:
                            break;
                        case CremaEvents.MessageReceived:
                            break;
                    }
                });
            }
        }

        private void Users_UsersStateChanged(object sender, ItemsEventArgs<IUser> e)
        {
            throw new NotImplementedException();
        }
    }
}
