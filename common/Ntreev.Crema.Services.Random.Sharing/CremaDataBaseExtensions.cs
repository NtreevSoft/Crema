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
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Linq;
using Ntreev.Library;
using System.Reflection;
using Ntreev.Crema.Services;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaDataBaseExtensions
    {
        private static readonly Dictionary<ActionTypes, int> changes = new Dictionary<ActionTypes, int>();

        static CremaDataBaseExtensions()
        {
            changes.Add(ActionTypes.Delete, 5);
        }

        public static bool IsExceptionOccurred { get; set; }

        public static ActionTargets ActionTargets { get; set; }

        public static ActionTypes ActionTypes { get; set; }

        public static IDictionary<ActionTypes, int> Changes
        {
            get { return changes; }
        }

        public static bool CanInvoke(ActionTypes actionType)
        {
            foreach (var item in changes)
            {
                if ((actionType & item.Key) != ActionTypes.None)
                {
                    return RandomUtility.Within(changes[actionType]);
                }
            }
            return true;
        }

#if !CLIENT
        //public static void CreateBase(this ICremaHost cremaHost)
        //{
        //    var authentication = cremaHost.Dispatcher.Invoke(() =>
        //    {
        //        var userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
        //        var user = userContext.Users.Random(item => item.Authority == Authority.Admin);
        //        return cremaHost.Login(user.ID, "admin".Encrypt());
        //    });

        //    var stoarge = cremaHost.GetService(typeof(IRepository)) as IRepository;
        //    stoarge.BeginTransaction(cremaHost.BasePath + "\\users.xml", "UserCreation");
        //    for (int i = 0; i < 100; i++)
        //    {
        //        CremaHostUtility.UserCreateTest(cremaHost, authentication);
        //    };
        //    stoarge.EndTransaction(cremaHost.BasePath + "\\users.xml");

        //    var dataBase = cremaHost.RandomDataBase();

        //    if (dataBase.TypeContext.Types.Any() == false)
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.TypeCategoryCreateTest(cremaHost, authentication);
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.TypeCreateTest(cremaHost, authentication);
        //        }
        //    }

        //    if (dataBase.TableContext.Tables.Any() == false)
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.TableCategoryCreateTest(cremaHost, authentication);
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.TableCreateTest(cremaHost, authentication);
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.TableInheritTest(cremaHost, authentication);
        //        }

        //        for (int i = 0; i < 10; i++)
        //        {
        //            CremaHostUtility.ChildTableCreateTest(cremaHost, authentication);
        //        }
        //    }

        //    cremaHost.Dispatcher.Invoke(() =>
        //    {
        //        var userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
        //        userContext.Logout(authentication, authentication.UserID);
        //    });
        //}
#endif
        [ActionTarget(ActionTargets.User)]
        [ActionType(ActionTypes.Create)]
        public static void UserCreateTest(this ICremaHost cremaHost, Authentication authentication)
        {
            UserCreateTest(cremaHost, authentication, (Authority)RandomUtility.Next(typeof(Authority)));
        }

        [ActionTarget(ActionTargets.User)]
        [ActionType(ActionTypes.Create)]
        public static void UserCreateTest(this ICremaHost cremaHost, Authentication authentication, Authority authority)
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
                if (userContext.Users[authentication.ID].Authority != Authority.Admin)
                    return;

                var category = userContext.Categories.Random();
                var identifier = RandomUtility.NextIdentifier();

                if (authority == Authority.Admin)
                {
                    var newID = string.Format("Admin_{0}", identifier);
                    var newName = string.Format("관리자_{0}", identifier);

                    category.AddNewUser(authentication, newID, null, newName, Authority.Admin);
                }
                else if (authority == Authority.Member)
                {
                    var newID = string.Format("Member_{0}", identifier);
                    var newName = string.Format("구성원_{0}", identifier);

                    category.AddNewUser(authentication, newID, null, newName, Authority.Member);
                }
                else
                {
                    var newID = string.Format("Guest_{0}", identifier);
                    var newName = string.Format("손님_{0}", identifier);

                    category.AddNewUser(authentication, newID, null, newName, Authority.Guest);
                }
            });
        }

        [ActionTarget(ActionTargets.Type)]
        [ActionType(ActionTypes.Create)]
        public static void TypeCreateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var category = typeContext.Categories.RandomOrDefault();
                typeContext.AddRandomType(authentication);
            });
        }

        [ActionTarget(ActionTargets.Type)]
        [ActionType(ActionTypes.Rename)]
        public static void TypeRenameTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var type = typeContext.Types.RandomOrDefault();

                if (type == null)
                    return;

                if (type.VerifyAccessType(authentication, AccessType.Master) == false)
                    return;

                var newName = string.Join("_", "Type", RandomUtility.NextIdentifier());

                if (typeContext.Types.Contains(newName) == true)
                    return;

                type.Rename(authentication, newName);
            });
        }

        [ActionTarget(ActionTargets.Type)]
        [ActionType(ActionTypes.Move)]
        public static void TypeMoveTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var type = typeContext.Types.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));

                if (type == null)
                    return;

                if (type.VerifyAccessType(authentication, AccessType.Master) == false)
                    return;

                var category = typeContext.Categories.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));
                if (category == null || type.Category == category)
                    return;

                type.Move(authentication, category.Path);
            });
        }

        [ActionTarget(ActionTargets.Type)]
        [ActionType(ActionTypes.Delete)]
        public static void TypeDeleteTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var type = typeContext.Types.RandomOrDefault();

                if (type == null)
                    return;

                var category = typeContext.Categories.Random();
                type.Delete(authentication);
            });
        }

        [ActionTarget(ActionTargets.TypeTemplate)]
        [ActionType(ActionTypes.Change)]
        public static void TypeTemplateEditTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var type = typeContext.Types.RandomOrDefault();
                if (type == null)
                    return;

                var template = type.Template;
                template.BeginEdit(authentication);

                try
                {
                    //template.EditMany(authentication, RandomUtility.Next(5, 10));
                    template.EndEdit(authentication);
                }
                catch
                {
                    template.CancelEdit(authentication);
                }
            });
        }

        [ActionTarget(ActionTargets.TypeCategory)]
        [ActionType(ActionTypes.Create)]
        public static void TypeCategoryCreateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                typeContext.AddRandomCategory(authentication);
            });
        }

        [ActionTarget(ActionTargets.TypeCategory)]
        [ActionType(ActionTypes.Rename)]
        public static void TypeCategoryRenameTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var category = typeContext.Categories.RandomOrDefault(item => item != typeContext.Root && item.VerifyAccessType(authentication, AccessType.Master));
                if (category == null)
                    return;

                var parent = category.Parent;
                var newName = RandomUtility.NextIdentifier();

                if (parent.Categories.ContainsKey(newName) == true)
                    return;

                category.Rename(authentication, newName);
            });
        }

        [ActionTarget(ActionTargets.TypeCategory)]
        [ActionType(ActionTypes.Move)]
        public static void TypeCategoryMoveTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var category = typeContext.Categories.RandomOrDefault(item => item != typeContext.Root && item.VerifyAccessType(authentication, AccessType.Master));
                if (category == null)
                    return;

                var descendants = EnumerableUtility.Descendants(category, item => item.Categories);
                var parent = typeContext.Categories.Random(item => descendants.Contains(item) == false && item.VerifyAccessType(authentication, AccessType.Master));

                if (category == parent || category.Parent == parent)
                    return;

                category.Move(authentication, parent.Path);
            });
        }

        [ActionTarget(ActionTargets.TypeCategory)]
        [ActionType(ActionTypes.Delete)]
        public static void TypeCategoryDeleteTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;
                var category = typeContext.Categories.RandomOrDefault(item => item != typeContext.Root && item.VerifyAccessType(authentication, AccessType.Master));

                if (category == null)
                    return;

                var types = EnumerableUtility.Descendants<IItem, IType>(category as IItem, item => item.Childs).ToArray();
                if (types.Any() == true)
                    return;

                category.Delete(authentication);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Lock)]
        public static void TypeItemLockTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Owner) && item.LockInfo.Path != item.Path);

                if (typeItem == null || typeItem.Parent == null)
                    return;

                var comment = RandomUtility.NextString();

                typeItem.Lock(authentication, comment);

                //Assert.IsTrue(typeItem.IsLocked);
                //Assert.AreEqual(typeItem.Path, typeItem.LockInfo.Path);
                //Assert.AreEqual(authentication.UserID, typeItem.LockInfo.UserID);
                //Assert.AreEqual(comment, typeItem.LockInfo.Comment);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Lock)]
        public static void TypeItemUnlockTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.LockInfo.UserID == authentication.ID && item.LockInfo.Path == item.Path);

                if (typeItem == null)
                    return;

                typeItem.Unlock(authentication);

                //Assert.AreEqual(string.Empty, typeItem.LockInfo.Path);
                //Assert.AreEqual(string.Empty, typeItem.LockInfo.UserID);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Access)]
        public static void TypeItemSetPrivateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master) && item.AccessInfo.Path != item.Path);

                if (typeItem == null || typeItem.Parent == null)
                    return;

                typeItem.SetPrivate(authentication);

                //Assert.IsTrue(typeItem.IsPrivate);
                //Assert.AreEqual(typeItem.Path, typeItem.AccessInfo.Path);
                //Assert.AreEqual(authentication.UserID, typeItem.AccessInfo.UserID);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Access)]
        public static void TypeItemSetPublicTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);

                if (typeItem == null)
                    return;

                typeItem.SetPublic(authentication);

                //Assert.AreEqual(string.Empty, typeItem.AccessInfo.Path);
                //Assert.AreEqual(string.Empty, typeItem.AccessInfo.UserID);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Access)]
        [Obsolete]
        public static void TypeItemAddAccessMemberTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var userContext = dataBase.GetService(typeof(IUserContext)) as IUserContext;
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);

                if (typeItem == null)
                    return;

                var user = userContext.Users.RandomOrDefault(item => item.ID != authentication.ID);

                typeItem.AddAccessMember(authentication, user.ID, AccessType.Owner);
            });
        }

        [ActionTarget(ActionTargets.TypeItem)]
        [ActionType(ActionTypes.Access)]
        public static void TypeItemRemoveAccessMemberTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var userContext = dataBase.GetService(typeof(IUserContext)) as IUserContext;
                var typeContext = dataBase.TypeContext;

                var typeItem = typeContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);

                if (typeItem == null)
                    return;

                if (typeItem.AccessInfo.Members.Any() == false)
                    return;

                var accessMember = typeItem.AccessInfo.Members.Random();
                typeItem.RemoveAccessMember(authentication, accessMember.UserID);
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Create)]
        public static void TableCreateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var category = tableContext.Categories.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));

                if (category == null)
                    return;

                var template = category.NewTable(authentication);
                try
                {
                    template.SetTags(authentication, CremaRandomUtility.RandomTags());
                    template.SetComment(authentication, RandomUtility.NextString());
                    template.GenerateColumns(authentication, RandomUtility.Next(1, 10));
                    template.EndEdit(authentication);
                }
                catch
                {
                    template.CancelEdit(authentication);
                }
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Rename)]
        public static void TableRenameTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tables = dataBase.GetService(typeof(ITableCollection)) as ITableCollection;
                var table = tables.RandomOrDefault(item => item.TemplatedParent == null);

                if (table == null)
                    return;

                var newName = NameUtility.GenerateNewName(RandomUtility.NextIdentifier(), tables.Select(item => item.Name));

                table.Rename(authentication, newName);
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Move)]
        public static void TableMoveTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));

                if (table == null)
                    return;

                var category = tableContext.Categories.Random(item => item.VerifyAccessType(authentication, AccessType.Master));
                if (table.Category == category)
                    return;

                table.Move(authentication, category.Path);
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Delete)]
        public static void TableDeleteTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));

                if (table == null)
                    return;

                table.Delete(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableTemplate)]
        [ActionType(ActionTypes.Change)]
        public static void TableSetTagsTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));

                if (table == null || table.TemplatedParent != null || table.TableState != TableState.None)
                    return;

                table.SetTags(authentication, CremaRandomUtility.RandomTags());
            });
        }

        [ActionTarget(ActionTargets.TableTemplate)]
        [ActionType(ActionTypes.Change)]
        public static void TableTemplateEditTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.TemplatedParent == null);

                if (table == null || table.TemplatedParent != null || table.TableState != TableState.None)
                    return;

                var template = table.Template;
                template.BeginEdit(authentication);

                try
                {
                    template.EditRandom(authentication, RandomUtility.Next(5, 10));
                    template.EndEdit(authentication);
                }
                catch
                {
                    template.CancelEdit(authentication);
                }
            });
        }

        //[ActionTarget(ActionTargets.TableContent)]
        //[ActionType(ActionTypes.Change)]
        public static void TableContentEditManyTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.Parent == null);

                if (table == null)
                    return;

                var content = table.Content;
                content.EnterEdit(authentication);

                var contents = EnumerableUtility.Friends(table, table.Childs).Select(item => item.Content);

                try
                {
                    contents.Random().EditRandom(authentication, RandomUtility.Next(1000));
                    content.LeaveEdit(authentication);
                }
                catch
                {
                    content.LeaveEdit(authentication);
                }
            });
        }

        [ActionTarget(ActionTargets.TableContent)]
        [ActionType(ActionTypes.Change)]
        [ActionWeight(5)]
        public static void TableContentEnterEditTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.Parent == null);

                if (table == null)
                    return;

                if (table.TableState.HasFlag(TableState.IsBeingEdited) == true && table.TableState.HasFlag(TableState.IsMember) == true)
                    return;

                var content = table.Content;
                content.EnterEdit(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableContent)]
        [ActionType(ActionTypes.Change)]
        [ActionWeight(5)]
        public static void TableContentLeaveEditTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.Parent == null && item.TableState.HasFlag(TableState.IsBeingEdited | TableState.IsMember));

                if (table == null)
                    return;

                table.Content.LeaveEdit(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableContent)]
        [ActionType(ActionTypes.Change)]
        public static void TableContentEditTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.Parent == null && item.TableState.HasFlag(TableState.IsBeingEdited | TableState.IsMember));

                if (table == null)
                    return;

                var content = table.Content;
                var contents = EnumerableUtility.Friends(table, table.Childs).Select(item => item.Content);
                contents.Random().EditRandom(authentication, 1);
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Create)]
        public static void ChildTableCreateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;

                var table = tableContext.Tables.RandomOrDefault(item => item.Parent == null);
                if (table == null || table.TemplatedParent != null)
                    return;

                var template = table.NewTable(authentication);
                try
                {
                    template.SetTags(authentication, CremaRandomUtility.RandomTags());
                    template.SetComment(authentication, RandomUtility.NextString());
                    template.GenerateColumns(authentication, RandomUtility.Next(1, 10));
                    template.EndEdit(authentication);
                }
                catch
                {
                    template.CancelEdit(authentication);
                }
            });
        }

        [ActionTarget(ActionTargets.TableCategory)]
        [ActionType(ActionTypes.Create)]
        public static void TableCategoryCreateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                tableContext.GenerateCategory(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableCategory)]
        [ActionType(ActionTypes.Rename)]
        public static void TableCategoryRenameTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var category = tableContext.Categories.RandomOrDefault(item => item != tableContext.Root);
                if (category == null)
                    return;

                var parent = category.Parent;
                var newName = RandomUtility.NextIdentifier();

                if (parent.Categories.ContainsKey(newName) == true)
                    return;

                category.Rename(authentication, newName);
            });
        }

        [ActionTarget(ActionTargets.TableCategory)]
        [ActionType(ActionTypes.Move)]
        public static void TableCategoryMoveTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var category = tableContext.Categories.RandomOrDefault(item => item != tableContext.Root);

                if (category == null)
                    return;

                var descendants = EnumerableUtility.Descendants(category, item => item.Categories);
                var parent = tableContext.Categories.Random(item => descendants.Contains(item) == false);

                if (category == parent || category.Parent == parent)
                    return;

                category.Move(authentication, parent.Path);
            });
        }

        [ActionTarget(ActionTargets.TableCategory)]
        [ActionType(ActionTypes.Delete)]
        public static void TableCategoryDeleteTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var category = tableContext.Categories.RandomOrDefault(item => item != tableContext.Root);

                if (category == null)
                    return;

                var tables = EnumerableUtility.Descendants<IItem, ITable>(category as IItem, item => item.Childs).ToArray();
                if (tables.Any() == true)
                    return;

                category.Delete(authentication);
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Inherit)]
        public static void TableInheritTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.TemplatedParent == null && item.VerifyAccessType(authentication, AccessType.Master) && item.Parent == null);

                if (table == null)
                    return;

                var newName = string.Join("_", "Table", RandomUtility.NextIdentifier());

                if (tableContext.Tables.Contains(newName) == true)
                    return;

                var category = tableContext.Categories.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master));
                if (category == null)
                    return;

                table.Inherit(authentication, newName, category.Path, RandomUtility.NextBoolean());
            });
        }

        [ActionTarget(ActionTargets.Table)]
        [ActionType(ActionTypes.Copy)]
        public static void TableCopyTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;
                var table = tableContext.Tables.RandomOrDefault(item => item.TemplatedParent == null && item.VerifyAccessType(authentication, AccessType.Guest) && item.Parent == null);

                if (table == null)
                    return;

                var newName = string.Join("_", "Table", RandomUtility.NextIdentifier());

                if (tableContext.Tables.Contains(newName) == true)
                    return;

                var category = tableContext.Categories.Random();
                table.Copy(authentication, newName, category.Path, RandomUtility.NextBoolean());
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Lock)]
        public static void TableItemLockTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Owner) && item.LockInfo.Path != item.Path);

                if (tableItem == null || tableItem.Parent == null)
                    return;

                var comment = RandomUtility.NextString();
                tableItem.Lock(authentication, comment);
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Lock)]
        public static void TableItemUnlockTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.LockInfo.UserID == authentication.ID && item.LockInfo.Path == item.Path);

                if (tableItem == null)
                    return;

                tableItem.Unlock(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Access)]
        public static void TableItemSetPrivateTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.VerifyAccessType(authentication, AccessType.Master) && item.AccessInfo.Path != item.Path);

                if (tableItem == null || tableItem.Parent == null)
                    return;

                tableItem.SetPrivate(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Access)]
        public static void TableItemSetPublicTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);
                if (tableItem == null)
                    return;

                tableItem.SetPublic(authentication);
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Access)]
        [Obsolete]
        public static void TableItemAddAccessMemberTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var userContext = dataBase.GetService(typeof(IUserContext)) as IUserContext;
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);

                if (tableItem == null)
                    return;

                var user = userContext.Users.RandomOrDefault(item => item.ID != authentication.ID);

                tableItem.AddAccessMember(authentication, user.ID, AccessType.Owner);
            });
        }

        [ActionTarget(ActionTargets.TableItem)]
        [ActionType(ActionTypes.Access)]
        public static void TableItemRemoveAccessMemberTest(this IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var userContext = dataBase.GetService(typeof(IUserContext)) as IUserContext;
                var tableContext = dataBase.TableContext;

                var tableItem = tableContext.RandomOrDefault(item => item.AccessInfo.UserID == authentication.ID && item.AccessInfo.Path == item.Path);

                if (tableItem == null)
                    return;

                if (tableItem.AccessInfo.Members.Any() == false)
                    return;

                var accessMember = tableItem.AccessInfo.Members.Random();
                tableItem.RemoveAccessMember(authentication, accessMember.UserID);
            });
        }

        [ActionTarget(ActionTargets.DataBase)]
        [ActionType(ActionTypes.Create)]
        public static void DataBaseCreateTest(this ICremaHost cremaHost, Authentication authentication)
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var dataBaseName = string.Join("_", "DataBase", RandomUtility.NextWord());
                cremaHost.DataBases.AddNewDataBase(authentication, dataBaseName, RandomUtility.NextString());
            });
        }

        public static void DataBaseEnterTest(this ICremaHost cremaHost, Authentication authentication)
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = cremaHost.RandomDataBase();
                if (dataBase.IsLoaded == false)
                    dataBase.Load(authentication);
                dataBase.Enter(authentication);
            });
        }

        public static void RandomTask(this ICremaHost cremaHost, Authentication authentication)
        {
            var methods = typeof(CremaDataBaseExtensions).GetMethods();
            var method = methods.Random(Predicate);
            method.Invoke(null, new object[] { cremaHost, authentication });
        }

        public static bool RandomTask(this IDataBase dataBase, Authentication authentication)
        {
            var methods = typeof(CremaDataBaseExtensions).GetMethods();
            var method = methods.RandomOrDefault((item) => Predicate(item, typeof(IDataBase)));
            var weightAttr = method.GetCustomAttribute<ActionWeightAttribute>();
            if (weightAttr != null && RandomUtility.Within(weightAttr.Weight) == false)
                return false;
            if (method == null)
                return false;
            method.Invoke(null, new object[] { dataBase, authentication });
            return true;
        }

        private static bool Predicate(MethodInfo methodInfo)
        {
            return Predicate(methodInfo, typeof(ICremaHost));
        }

        private static bool Predicate(MethodInfo methodInfo, Type type)
        {
            if (methodInfo.IsStatic == false)
                return false;

            if (methodInfo.Name == "RandomTask")
                return false;

            var targetAttr = methodInfo.GetCustomAttribute<ActionTargetAttribute>();
            if (targetAttr == null)
                return false;

            if (ActionTargets != ActionTargets.None && (ActionTargets & targetAttr.Targets) == ActionTargets.None)
                return false;

            var typeAttr = methodInfo.GetCustomAttribute<ActionTypeAttribute>();
            if (typeAttr == null)
                return false;

            if (ActionTypes != ActionTypes.None && (ActionTypes & typeAttr.Types) == ActionTypes.None)
                return false;

            var parameters = methodInfo.GetParameters();
            if (parameters.Count() != 2)
                return false;

            return parameters[0].ParameterType == type && parameters[1].ParameterType == typeof(Authentication);
        }

        private static void ValidateNotNull(ITypeItem typeItem)
        {
            if (IsExceptionOccurred == false)
                return;

            if (typeItem == null)
                throw new ArgumentNullException();

        }

        //private static void ValidateWrite(ITypeItem typeItem, Authentication authentication)
        //{
        //    if (IsExceptionOccurred == false)
        //        return;

        //    if (typeItem.VerifyWrite(authentication) == false)
        //        throw new PermissionDeniedException();
        //}
    }
}
