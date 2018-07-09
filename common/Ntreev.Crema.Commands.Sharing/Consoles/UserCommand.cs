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

using Ntreev.Library;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ntreev.Crema.ServiceModel;
using System.ComponentModel;
using Newtonsoft.Json;
using Ntreev.Library.IO;
using System.Diagnostics;
using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.Commands.Consoles.Serializations;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class UserCommand : ConsoleCommandMethodBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public UserCommand(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(IsOnline), nameof(IsBanned))]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void List()
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            var metaItems = this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.GetMetaData(authentication).Users
                            let userID = item.UserInfo.ID
                            orderby userID
                            where StringUtility.GlobMany(userID, FilterProperties.FilterExpression)
                            where this.IsOnline == false || item.UserState == UserState.Online
                            where this.IsBanned == false || item.BanInfo.Path == item.Path
                            select item;

                return query.ToArray();
            });

            this.Out.Print(metaItems, this.PrintItem, this.SelectItem);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Kick([CommandCompletion(nameof(GetOnlineUserIDs))]string userID)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.Kick(authentication, this.Comment));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Ban([CommandCompletion(nameof(GetUnbannedUserIDs))]string userID)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.Ban(authentication, this.Comment));
        }

        [CommandMethod]
        public void Unban([CommandCompletion(nameof(GetBannedUserIDs))]string userID)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.Unban(authentication));
        }

        [CommandMethod]
        public void Password([CommandCompletion(nameof(GetUserIDs))]string userID)
        {
            var user = this.GetUser(userID);
            var password1 = this.CommandContext.ReadSecureString("Password1:");
            var password2 = this.CommandContext.ReadSecureString("Password2:");
            ConsoleCommandContextBase.Validate(password1, password2);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.ChangeUserInfo(authentication, null, password1, null, null));
        }

        [CommandMethod]
        public void Rename([CommandCompletion(nameof(GetUser))]string userID, string newName = null)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            var newValue = newName ?? this.CommandContext.ReadString("NewName:");
            user.Dispatcher.Invoke(() => user.ChangeUserInfo(authentication, null, null, newValue, null));
        }

        [CommandMethod]
        public void Move([CommandCompletion(nameof(GetUser))]string userID, string categoryPath)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.Move(authentication, categoryPath));
        }

        [CommandMethod("authority")]
        public void SetAuthority([CommandCompletion(nameof(GetUser))]string userID, Authority authority)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            user.Dispatcher.Invoke(() => user.ChangeUserInfo(authentication, null, null, null, authority));
        }

        [CommandMethod]
        public void Info([CommandCompletion(nameof(GetUser))]string userID)
        {
            var user = this.GetUser(userID);
            var userInfo = user.Dispatcher.Invoke(() => user.UserInfo);
            var props = userInfo.ToDictionary();
            var text = TextSerializer.Serialize(props);
            this.Out.WriteLine(text);
        }

        [ConsoleModeOnly]
        [CommandMethod]
        [CommandMethodProperty(nameof(CategoryPath))]
        public void Create()
        {
            var schema = JsonSchemaUtility.CreateSchema(typeof(JsonUserInfo));
            schema.SetEnums(nameof(JsonUserInfo.CategoryPath), this.GetCategoryPaths());

            var userInfo = JsonUserInfo.Default;
            userInfo.CategoryPath = this.CategoryPath;
            if (JsonEditorHost.TryEdit(ref userInfo, schema) == false)
                return;

            var category = this.GetCategory(this.CategoryPath ?? this.CommandContext.Path);
            var userID = userInfo.UserID;
            var password = StringUtility.ToSecureString(userInfo.Password);
            var userName = userInfo.UserName;
            var authority = (Authority)Enum.Parse(typeof(Authority), userInfo.Authority);
            var authentication = this.CommandContext.GetAuthentication(this);
            category.Dispatcher.Invoke(() => category.AddNewUser(authentication, userID, password, userName, authority));
        }

        [CommandMethod]
        public void Delete([CommandCompletion(nameof(GetUser))]string userID)
        {
            var user = this.GetUser(userID);
            var authentication = this.CommandContext.GetAuthentication(this);
            if (this.CommandContext.ConfirmToDelete() == true)
            {
                user.Dispatcher.Invoke(() => user.Delete(authentication));
            }
        }

        [CommandMethod]
        public void Message([CommandCompletion(nameof(GetUser))]string userID, string message)
        {
            this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var authentication = this.CommandContext.GetAuthentication(this);
                user.SendMessage(authentication, message);
            });
        }

        [CommandProperty("online", 'o')]
        public bool IsOnline
        {
            get; set;
        }

        [CommandProperty("banned")]
        public bool IsBanned
        {
            get; set;
        }

        [CommandProperty('m', IsRequired = true)]
        public string Comment
        {
            get; set;
        }

        [CommandProperty]
        [CommandCompletion(nameof(GetCategoryPaths))]
        public string CategoryPath
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.Drive is UsersConsoleDrive;

        protected IUserContext UserContext
        {
            get { return this.cremaHost.GetService(typeof(IUserContext)) as IUserContext; }
        }

        private IUser GetUser(string userID)
        {
            var user = this.UserContext.Dispatcher.Invoke(() => this.UserContext.Users[userID]);
            if (user == null)
                throw new UserNotFoundException(userID);
            return user;
        }

        private IUserCategory GetCategory(string categoryPath)
        {
            var category = this.UserContext.Dispatcher.Invoke(() => this.UserContext.Categories[categoryPath]);
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            return category;
        }

        private void PrintItem(UserMetaData item, Action action)
        {
            if (item.BanInfo.Path != string.Empty)
            {
                using (TerminalColor.SetForeground(ConsoleColor.Red))
                {
                    action();
                }
            }
            else if (item.UserState != UserState.Online)
            {
                //using (TerminalColor.SetForeground(ConsoleColor.Gray))
                {
                    action();
                }
            }
            else
            {
                using (TerminalColor.SetForeground(ConsoleColor.Blue))
                {
                    action();
                }
            }
        }

        private string SelectItem(UserMetaData item)
        {
            return item.UserInfo.ID;
        }

        private string[] GetUserIDs()
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Users
                            orderby item.ID
                            select item.ID;
                return query.ToArray();
            });
        }

        private string[] GetCategoryPaths()
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Categories
                            orderby item.Path
                            select item.Path;
                return query.ToArray();
            });
        }

        private string[] GetOnlineUserIDs()
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Users
                            where item.UserState.HasFlag(UserState.Online)
                            select item.ID;
                return query.ToArray();
            });
        }

        private string[] GetUnbannedUserIDs()
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Users
                            where item.BanInfo.Path != item.Path
                            select item.ID;
                return query.ToArray();
            });
        }

        private string[] GetBannedUserIDs()
        {
            return this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Users
                            where item.BanInfo.Path == item.Path
                            select item.ID;
                return query.ToArray();
            });
        }
    }
}
