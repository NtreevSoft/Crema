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
            : base("user")
        {
            this.cremaHost = cremaHost;
        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            switch (methodDescriptor.DescriptorName)
            {
                case nameof(Kick):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
                        {
                            return this.UserContext.Dispatcher.Invoke(() =>
                            {
                                var query = from item in this.UserContext.Users
                                            where item.UserState.HasFlag(UserState.Online)
                                            select item.ID;
                                return query.ToArray();
                            });
                        }
                    }
                    break;
                case nameof(Ban):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
                        {
                            return this.UserContext.Dispatcher.Invoke(() =>
                            {
                                var query = from item in this.UserContext.Users
                                            where item.BanInfo.Path != item.Path
                                            select item.ID;
                                return query.ToArray();
                            });
                        }
                    }
                    break;
                case nameof(Unban):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
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
                    break;
                case nameof(Move):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
                        {
                            if (memberDescriptor.DescriptorName == "userID")
                            {
                                return this.UserContext.Dispatcher.Invoke(() =>
                                {
                                    var query = from item in this.UserContext.Users
                                                select item.ID;
                                    return query.ToArray();
                                });
                            }
                            else if (memberDescriptor.DescriptorName == "categoryPath")
                            {
                                return this.UserContext.Dispatcher.Invoke(() =>
                                {
                                    var query = from item in this.UserContext.Categories
                                                select item.Path;
                                    return query.ToArray();
                                });
                            }
                        }
                    }
                    break;
                case nameof(Create):
                    {
                        if (memberDescriptor.DescriptorName == "categoryPath")
                        {
                            return this.UserContext.Dispatcher.Invoke(() =>
                            {
                                var query = from item in this.UserContext.Categories
                                            select item.Path;
                                return query.ToArray();
                            });
                        }
                    }
                    break;
                case nameof(Info):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
                        {
                            return this.UserContext.Dispatcher.Invoke(() =>
                            {
                                var query = from item in this.UserContext.Users
                                            select item.ID;
                                return query.ToArray();
                            });
                        }
                    }
                    break;
                case nameof(Rename):
                case nameof(SetAuthority):
                case nameof(Delete):
                case nameof(Message):
                    {
                        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "userID")
                        {
                            return this.UserContext.Dispatcher.Invoke(() =>
                            {
                                var query = from item in this.UserContext.Users
                                            select item.ID;
                                return query.ToArray();
                            });
                        }
                    }
                    break;
                    //case nameof(CreateCategory):
                    //    {
                    //        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "categoryPath")
                    //        {
                    //            return this.GetCategoryPaths();
                    //        }
                    //    }
                    //    break;
                    //case nameof(RenameCategory):
                    //    {
                    //        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "categoryPath")
                    //        {
                    //            return this.GetCategoryPaths();
                    //        }
                    //    }
                    //    break;
                    //case nameof(MoveCategory):
                    //    {
                    //        if (memberDescriptor is CommandParameterDescriptor)
                    //        {
                    //            if (memberDescriptor.DescriptorName == "categoryPath" || memberDescriptor.DescriptorName == "parentPath")
                    //            {
                    //                return this.GetCategoryPaths();
                    //            }
                    //        }
                    //    }
                    //    break;
                    //case nameof(DeleteCategory):
                    //    {
                    //        if (memberDescriptor is CommandParameterDescriptor && memberDescriptor.DescriptorName == "categoryPath")
                    //        {
                    //            return this.GetCategoryPaths();
                    //        }
                    //    }
                    //    break;
            }
            return null;
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
        public void ListCategory()
        {
            var items = this.UserContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.UserContext.Categories
                            orderby item.Path
                            select item.Path;

                return query.ToArray();
            });

            foreach (var item in items)
            {
                this.Out.WriteLine(item);
            }
        }

        [CommandMethod]
        public void Kick(string userID, string comment)
        {
            this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var authentication = this.CommandContext.GetAuthentication(this);
                user.Kick(authentication, comment);
            });
        }

        [CommandMethod]
        public void Ban(string userID, string comment)
        {
            this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var authentication = this.CommandContext.GetAuthentication(this);
                user.Ban(authentication, comment);
            });
        }

        [CommandMethod]
        public void Unban(string userID)
        {
            this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var authentication = this.CommandContext.GetAuthentication(this);
                user.Unban(authentication);
            });
        }

        [CommandMethod]
        public void Password(string userID)
        {
            var user = this.UserContext.Dispatcher.Invoke(() => this.GetUser(userID));
            var password1 = this.CommandContext.ReadSecureString("Password1:");
            var password2 = this.CommandContext.ReadSecureString("Password2:");
            ConsoleCommandContextBase.Validate(password1, password2);

            this.UserContext.Dispatcher.Invoke(() =>
            {
                var authentication = this.CommandContext.GetAuthentication(this);
                user.ChangeUserInfo(authentication, null, password1, null, null);
            });
        }

        [CommandMethod]
        public void Rename(string userID, string newName = null)
        {
            var user = this.UserContext.Dispatcher.Invoke(() => this.GetUser(userID));
            var authentication = this.CommandContext.GetAuthentication(this);

            if (newName == null)
                newName = this.CommandContext.ReadString("NewName:");

            this.UserContext.Dispatcher.Invoke(() =>
            {
                
                user.ChangeUserInfo(authentication, null, null, newName, null);
            });
        }

        [CommandMethod]
        public void Move(string userID, string categoryPath)
        {
            var user = this.UserContext.Dispatcher.Invoke(() => this.GetUser(userID));
            var authentication = this.CommandContext.GetAuthentication(this);

            this.UserContext.Dispatcher.Invoke(() =>
            {
                user.Move(authentication, categoryPath);
            });
        }

        [CommandMethod("authority")]
        public void SetAuthority(string userID, Authority authority)
        {
            this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                var authentication = this.CommandContext.GetAuthentication(this);
                user.ChangeUserInfo(authentication, null, null, null, authority);
            });
        }

        [CommandMethod]
        public void Info(string userID)
        {
            var userInfo = this.UserContext.Dispatcher.Invoke(() =>
            {
                var user = this.GetUser(userID);
                return user.UserInfo;
            });

            var items = new Dictionary<string, object>
            {
                { $"{nameof(userInfo.ID)}", userInfo.ID },
                { $"{nameof(userInfo.Name)}", userInfo.Name },
                { $"{nameof(userInfo.CategoryPath)}", userInfo.CategoryPath },
                { $"{nameof(userInfo.Authority)}", userInfo.Authority },
                { $"{nameof(userInfo.CreationInfo)}", userInfo.CreationInfo.ToLocalValue() },
                { $"{nameof(userInfo.ModificationInfo)}", userInfo.ModificationInfo.ToLocalValue() }
            };
            this.Out.Print<object>(items);
        }

        [ConsoleModeOnly]
        [CommandMethod]
        public void Create(string categoryPath = null)
        {
            var schema = JsonSchemaUtility.CreateSchema(typeof(JsonUserInfo));
            schema.SetEnums(nameof(JsonUserInfo.CategoryPath), this.GetCategoryPaths());

            var userInfo = JsonUserInfo.Default;
            userInfo.CategoryPath = categoryPath;
            if (JsonEditorHost.TryEdit(ref userInfo, schema) == false)
                return;

            this.UserContext.Dispatcher.Invoke(() =>
            {
                categoryPath = userInfo.CategoryPath ?? this.CommandContext.Path;
                var category = this.UserContext.Dispatcher.Invoke(() => this.GetCategory(categoryPath));
                var userID = userInfo.UserID;
                var password = StringUtility.ToSecureString(userInfo.Password);
                var userName = userInfo.UserName;
                var authority = (Authority)Enum.Parse(typeof(Authority), userInfo.Authority);
                var authentication = this.CommandContext.GetAuthentication(this);

                category.AddNewUser(authentication, userID, password, userName, authority);
            });
        }

        [CommandMethod]
        public void Delete(string userID)
        {
            var user = this.UserContext.Dispatcher.Invoke(() => this.GetUser(userID));
            if (this.CommandContext.ConfirmToDelete() == false)
            {
                this.Out.WriteLine("deletion has been cancelled.");
                return;
            }
            var authentication = this.CommandContext.GetAuthentication(this);
            this.UserContext.Dispatcher.Invoke(() => user.Delete(authentication));
        }

        [CommandMethod]
        public void Message(string userID, string message)
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

        public override bool IsEnabled => this.CommandContext.Drive is UsersConsoleDrive;

        protected IUserContext UserContext
        {
            get { return this.cremaHost.GetService(typeof(IUserContext)) as IUserContext; }
        }

        private IUser GetUser(string userID)
        {
            var user = this.UserContext.Users[userID];
            if (user == null)
                throw new UserNotFoundException(userID);
            return user;
        }

        private IUserCategory GetCategory(string categoryPath)
        {
            var category = this.UserContext.Categories[categoryPath];
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

        //private string GetCurrentDirectory()
        //{
        //    if (this.CommandContext.Drive is UsersConsoleDrive drive)
        //    {
        //        return this.CommandContext.Path;
        //    }
        //    return PathUtility.Separator;
        //}
    }
}
