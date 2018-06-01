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

using Ntreev.Crema.ServiceModel.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services.Data;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Services
{
    static class EventMessageBuilder
    {
        public static string LockDataBase(Authentication authentication, IDataBase[] items, string[] comments)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.LockDataBase, authentication.ID, authentication.Name, items[i].Name, comments[i]);
            }
            return sb.ToString();
        }

        public static string UnlockDataBase(Authentication authentication, IDataBase[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.UnlockDataBase, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPrivateDataBase(Authentication authentication, IDataBase[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPrivateDataBase, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPublicDataBase(Authentication authentication, IDataBase[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPublicDataBase, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string AddAccessMemberToDataBase(Authentication authentication, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.AddAccessMemberToDataBase, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string SetAccessMemberOfDataBase(Authentication authentication, IDataBase[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetAccessMemberOfDataBase, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string RemoveAccessMemberFromDataBase(Authentication authentication, IDataBase[] items, string[] memberIDs)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RemoveAccessMemberFromDataBase, authentication.ID, authentication.Name, items[i].Name, memberIDs[i]);
            }
            return sb.ToString();
        }

        public static string CreateDataBase(Authentication authentication, string[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string RenameDataBase(Authentication authentication, string[] dataBases, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameDataBase, authentication.ID, authentication.Name, oldNames[i], dataBases[i]);
            }
            return sb.ToString();
        }

        public static string DeleteDataBase(Authentication authentication, string[] dataBaseNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBaseNames.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteDataBase, authentication.ID, authentication.Name, dataBaseNames[i]);
            }
            return sb.ToString();
        }

        public static string LoadDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.LoadDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string UnloadDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.UnloadDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string ResettingDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ResettingDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string ResetDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ResetDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string EnterDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.EnterDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string LeaveDataBase(Authentication authentication, IDataBase[] dataBases)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < dataBases.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.LeaveDataBase, authentication.ID, authentication.Name, dataBases[i]);
            }
            return sb.ToString();
        }

        public static string RevertDataBase(Authentication authentication, DataBase dataBase, string revision)
        {
            return string.Format(EventResources.RevertDataBase, authentication.ID, authentication.Name, dataBase, revision);
        }

        public static string LockTypeItem(Authentication authentication, ITypeItem[] items, string[] comments)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.LockTypeItem, authentication.ID, authentication.Name, items[i].Name, comments[i]);
            }
            return sb.ToString();
        }

        public static string UnlockTypeItem(Authentication authentication, ITypeItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.UnlockTypeItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPrivateTypeItem(Authentication authentication, ITypeItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPrivateTypeItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPublicTypeItem(Authentication authentication, ITypeItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPublicTypeItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string AddAccessMemberToTypeItem(Authentication authentication, ITypeItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.AddAccessMemberToTypeItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string SetAccessMemberOfTypeItem(Authentication authentication, ITypeItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetAccessMemberOfTypeItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string RemoveAccessMemberFromTypeItem(Authentication authentication, ITypeItem[] items, string[] memberIDs)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RemoveAccessMemberFromTypeItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i]);
            }
            return sb.ToString();
        }

        public static string CreateTypeCategory(Authentication authentication, string[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateTypeCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string RenameTypeCategory(Authentication authentication, string[] categories, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameTypeCategory, authentication.ID, authentication.Name, oldNames[i], categories[i]);
            }
            return sb.ToString();
        }

        public static string MoveTypeCategory(Authentication authentication, string[] categories, string[] parentPaths, string[] oldParentPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveTypeCategory, authentication.ID, authentication.Name, categories[i], oldParentPaths[i], parentPaths);
            }
            return sb.ToString();
        }

        public static string DeleteTypeCategory(Authentication authentication, string[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteTypeCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string CreateType(Authentication authentication, string[] types)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateType, authentication.ID, authentication.Name, types[i]);
            }
            return sb.ToString();
        }

        public static string RenameType(Authentication authentication, string[] types, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameType, authentication.ID, authentication.Name, oldNames[i], types[i]);
            }
            return sb.ToString();
        }

        public static string MoveType(Authentication authentication, string[] types, string[] categoryPaths, string[] oldCategoryPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveType, authentication.ID, authentication.Name, types[i], oldCategoryPaths[i], categoryPaths[i]);
            }
            return sb.ToString();
        }

        public static string DeleteType(Authentication authentication, string[] types)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteType, authentication.ID, authentication.Name, types[i]);
            }
            return sb.ToString();
        }

        public static string ChangeTypeTemplate(Authentication authentication, string[] types)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ChangeTypeTemplate, authentication.ID, authentication.Name, types[i]);
            }
            return sb.ToString();
        }

        public static string CreateTableCategory(Authentication authentication, string[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateTableCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string LockTableItem(Authentication authentication, ITableItem[] items, string[] comments)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.LockTableItem, authentication.ID, authentication.Name, items[i].Name, comments[i]);
            }
            return sb.ToString();
        }

        public static string UnlockTableItem(Authentication authentication, ITableItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.UnlockTableItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPrivateTableItem(Authentication authentication, ITableItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPrivateTableItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string SetPublicTableItem(Authentication authentication, ITableItem[] items)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetPublicTableItem, authentication.ID, authentication.Name, items[i].Name);
            }
            return sb.ToString();
        }

        public static string AddAccessMemberToTableItem(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.AddAccessMemberToTableItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string SetAccessMemberOfTableItem(Authentication authentication, ITableItem[] items, string[] memberIDs, AccessType[] accessTypes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.SetAccessMemberOfTableItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i], accessTypes[i]);
            }
            return sb.ToString();
        }

        public static string RemoveAccessMemberFromTableItem(Authentication authentication, ITableItem[] items, string[] memberIDs)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RemoveAccessMemberFromTableItem, authentication.ID, authentication.Name, items[i].Name, memberIDs[i]);
            }
            return sb.ToString();
        }

        public static string RenameTableCategory(Authentication authentication, string[] categories, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameTableCategory, authentication.ID, authentication.Name, oldNames[i], categories[i]);
            }
            return sb.ToString();
        }

        public static string MoveTableCategory(Authentication authentication, string[] categories, string[] parentPaths, string[] oldParentPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveTableCategory, authentication.ID, authentication.Name, categories[i], oldParentPaths[i], parentPaths);
            }
            return sb.ToString();
        }

        public static string DeleteTableCategory(Authentication authentication, string[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteTableCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string CreateTable(Authentication authentication, string[] tables)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateTable, authentication.ID, authentication.Name, tables[i]);
            }
            return sb.ToString();
        }

        public static string RenameTable(Authentication authentication, string[] tables, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameTable, authentication.ID, authentication.Name, oldNames[i], tables[i]);
            }
            return sb.ToString();
        }

        public static string MoveTable(Authentication authentication, string[] tables, string[] categoryPaths, string[] oldCategoryPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveTable, authentication.ID, authentication.Name, tables[i], oldCategoryPaths[i], categoryPaths[i]);
            }
            return sb.ToString();
        }

        public static string DeleteTable(Authentication authentication, string[] tables)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteTable, authentication.ID, authentication.Name, tables[i]);
            }
            return sb.ToString();
        }

        public static string ChangeTableTemplate(Authentication authentication, string[] tables)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ChangeTableTemplate, authentication.ID, authentication.Name, tables[i]);
            }
            return sb.ToString();
        }

        public static string ChangeTableContent(Authentication authentication, string[] tables)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tables.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ChangeTableContent, authentication.ID, authentication.Name, tables[i]);
            }
            return sb.ToString();
        }

        public static string CreateUserCategory(Authentication authentication, IUserCategory[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateUserCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string RenameUserCategory(Authentication authentication, IUserCategory[] categories, string[] oldNames)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.RenameUserCategory, authentication.ID, authentication.Name, oldNames[i], categories[i].Name);
            }
            return sb.ToString();
        }

        public static string MoveUserCategory(Authentication authentication, IUserCategory[] categories, string[] oldParentPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveUserCategory, authentication.ID, authentication.Name, categories[i], oldParentPaths[i], categories[i].Parent);
            }
            return sb.ToString();
        }

        public static string DeleteUserCategory(Authentication authentication, IUserCategory[] categories)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < categories.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteUserCategory, authentication.ID, authentication.Name, categories[i]);
            }
            return sb.ToString();
        }

        public static string CreateUser(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.CreateUser, authentication.ID, authentication.Name, users[i], users[i].UserName);
            }
            return sb.ToString();
        }

        public static string ChangeUserInfo(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.ChangeUserInfo, authentication.ID, authentication.Name, users[i], users[i].UserName);
            }
            return sb.ToString();
        }

        public static string MoveUser(Authentication authentication, IUser[] users, string[] oldCategoryPaths)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.MoveUser, authentication.ID, authentication.Name, users[i], users[i].UserName, oldCategoryPaths[i], users[i].Category);
            }
            return sb.ToString();
        }

        public static string DeleteUser(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.DeleteUser, authentication.ID, authentication.Name, users[i]);
            }
            return sb.ToString();
        }

        public static string KickUser(Authentication authentication, IUser[] users, string[] comments)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.KickUser, authentication.ID, authentication.Name, users[i], users[i].UserName);
                sb.AppendLine();
                sb.Append($"{EventResources.Reason}: {comments[i]}");
            }
            return sb.ToString();
        }

        public static string BanUser(Authentication authentication, IUser[] users, string[] comments)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.BanUser, authentication.ID, authentication.Name, users[i], users[i].UserName);
                sb.AppendLine();
                sb.Append($"{EventResources.Reason}: {comments[i]}");
            }
            return sb.ToString();
        }

        public static string UnbanUser(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                sb.AppendFormat(EventResources.UnbanUser, authentication.ID, authentication.Name, users[i], users[i].UserName);
            }
            return sb.ToString();
        }

        public static string LoginUser(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                var user = users[i];
                var userInfo = user.UserInfo;
                sb.AppendFormat(EventResources.LoginUser, userInfo.ID, userInfo.Name);
            }
            return sb.ToString();
        }

        public static string LogoutUser(Authentication authentication, IUser[] users)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < users.Length; i++)
            {
                if (i > 0)
                    sb.AppendLine();
                var user = users[i];
                var userInfo = user.UserInfo;
                sb.AppendFormat(EventResources.LogoutUser, userInfo.ID, userInfo.Name);
            }
            return sb.ToString();
        }

        public static string BeginDomain(Authentication authentication, IDomain domain)
        {
            return string.Format(EventResources.BeginDomain, authentication.ID, authentication.Name, domain);
        }

        public static string EndDomain(Authentication authentication, IDomain domain)
        {
            return string.Format(EventResources.EndDomain, authentication.ID, authentication.Name, domain);
        }

        public static string CancelDomain(Authentication authentication, IDomain domain)
        {
            return string.Format(EventResources.CancelDomain, authentication.ID, authentication.Name, domain);
        }

        public static string EnterDomainUser(Authentication authentication, IDomain domain)
        {
            return string.Format(EventResources.EnterDomainUser, authentication.ID, authentication.Name, domain);
        }

        public static string LeaveDomainUser(Authentication authentication, IDomain domain)
        {
            return string.Format(EventResources.LeaveDomainUser, authentication.ID, authentication.Name, domain);
        }

        [Obsolete]
        public static string SetDomainOwner(Authentication authentication, IDomain domain, IDomainUser domainUser)
        {
            return string.Format(EventResources.SetDomainOwner, authentication.ID, authentication.Name, domainUser, domainUser.DomainUserInfo.UserName, domain);
        }

        public static string KickDomainUser(Authentication authentication, IDomain domain, IDomainUser domainUser)
        {
            return string.Format(EventResources.KickDomainUser, authentication.ID, authentication.Name, domainUser, domainUser.DomainUserInfo.UserName, domain);
        }

        public static string SendMessage(Authentication authentication, IUser user, string message)
        {
            return string.Format(EventResources.SendMessage, authentication.ID, authentication.Name, user.ID, user.UserName, message);
        }

        public static string NotifyMessage(Authentication authentication, IUser[] users, string message)
        {
            var target = users.Any() == false ? "all users" : string.Join(",", users.Select(item => item.ID).ToArray());
            return string.Format(EventResources.NotifyMessage, authentication.ID, authentication.Name, target, message);
        }
    }
}
