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
using Ntreev.Library.IO;
using Ntreev.Library;
using System.Text.RegularExpressions;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleDrive))]
    public sealed class DataBasesConsoleDrive : ConsoleDriveBase, IPartImportsSatisfiedNotification
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;
        private DataBasePath dataBasePath;

        internal DataBasesConsoleDrive()
            : base("databases")
        {

        }

        public override string[] GetPaths()
        {
            return this.CremaHost.Dispatcher.Invoke(GetPathsImpl);

            string[] GetPathsImpl()
            {
                var pathList = new List<string>
                {
                    PathUtility.Separator
                };
                foreach (var item in this.CremaHost.DataBases)
                {
                    var items = item.DataBaseInfo.Paths.Select(i => $"{PathUtility.SeparatorChar}{item.Name}{i}");
                    pathList.AddRange(items);
                }
                return pathList.ToArray();
            }
        }

        public string DataBaseName => this.dataBasePath?.DataBaseName ?? string.Empty;

        public string Context => this.dataBasePath?.Context ?? string.Empty;

        public string ItemPath => this.dataBasePath?.ItemPath ?? string.Empty;

        protected override void OnSetPath(Authentication authentication, string path)
        {
            var dataBaseName = this.DataBaseName;
            var dataBasePath = new DataBasePath(path);
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                if (dataBaseName != string.Empty && dataBasePath.DataBaseName != dataBaseName)
                {
                    var dataBase = this.CremaHost.DataBases[dataBaseName];
                    dataBase.Unloaded -= DataBase_Unloaded;
                    if (dataBase.IsLoaded == true)
                    {
                        dataBase.Leave(authentication);
                    }
                }

                if (dataBasePath.DataBaseName != string.Empty && dataBasePath.DataBaseName != dataBaseName)
                {
                    var dataBase = this.CremaHost.DataBases[dataBasePath.DataBaseName];
                    if (dataBase.IsLoaded == false)
                        dataBase.Load(authentication);
                    dataBase.Enter(authentication);
                    dataBase.Unloaded += DataBase_Unloaded;
                }
            });
            this.dataBasePath = dataBasePath;
        }

        protected override void OnCreate(Authentication authentication, string path, string name)
        {
            var target = this.GetObject(authentication, path);

            if (target is ITableCategory tableCategory)
            {
                this.CremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBase = tableCategory.GetService(typeof(IDataBase)) as IDataBase;
                    using (DataBaseUsing.Set(dataBase, authentication))
                    {
                        tableCategory.AddNewCategory(authentication, name);
                    }
                });
            }
            else if (target is ITypeCategory typeCategory)
            {
                this.CremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBase = typeCategory.GetService(typeof(IDataBase)) as IDataBase;
                    using (DataBaseUsing.Set(dataBase, authentication))
                    {
                        typeCategory.AddNewCategory(authentication, name);
                    }
                });
            }
            else if (path == PathUtility.Separator)
            {
                var comment = this.CommandContext.ReadString("comment:");
                this.CremaHost.Dispatcher.Invoke(() =>
                {
                    this.CremaHost.DataBases.AddNewDataBase(authentication, name, comment);
                });
            }
            else
            {
                var dataBasePath = new DataBasePath(path);
                if (dataBasePath.Context == string.Empty)
                    throw new PermissionDeniedException();
                throw new CategoryNotFoundException(path);
            }
        }

        protected override void OnMove(Authentication authentication, string path, string newPath)
        {
            var sourceObject = this.GetObject(authentication, path);

            if (sourceObject is IType sourceType)
            {
                this.MoveType(authentication, sourceType, newPath);
            }
            else if (sourceObject is ITypeCategory sourceTypeCategory)
            {
                this.MoveTypeCategory(authentication, sourceTypeCategory, newPath);
            }
            else if (sourceObject is ITable sourceTable)
            {
                this.MoveTable(authentication, sourceTable, newPath);
            }
            else if (sourceObject is ITableCategory sourceTableCategory)
            {
                this.MoveTableCategory(authentication, sourceTableCategory, newPath);
            }
            else if (sourceObject is IDataBase dataBase)
            {
                this.MoveDataBase(authentication, dataBase, newPath);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected override void OnDelete(Authentication authentication, string path)
        {
            var target = this.GetObject(authentication, path);
            this.CremaHost.Dispatcher.Invoke(Delete);

            void Delete()
            {
                if (target is IDataBase)
                {
                    var dataBase = target as IDataBase;
                    dataBase.Delete(authentication);
                }
                else if (target is ITableItem tableItem)
                {
                    var dataBase = tableItem.GetService(typeof(IDataBase)) as IDataBase;
                    using (DataBaseUsing.Set(dataBase, authentication))
                    {
                        tableItem.Delete(authentication);
                    }
                }
                else if (target is ITypeItem typeItem)
                {
                    var dataBase = typeItem.GetService(typeof(IDataBase)) as IDataBase;
                    using (DataBaseUsing.Set(dataBase, authentication))
                    {
                        typeItem.Delete(authentication);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private void MoveTypeCategory(Authentication authentication, ITypeCategory sourceCategory, string newPath)
        {
            var destPath = new DataBasePath(newPath);
            var destObject = this.GetObject(authentication, destPath.Path);
            this.CremaHost.Dispatcher.Invoke(MoveTypeCategory);

            void MoveTypeCategory()
            {
                var dataBase = sourceCategory.GetService(typeof(IDataBase)) as IDataBase;
                var types = sourceCategory.GetService(typeof(ITypeCollection)) as ITypeCollection;

                if (destPath.DataBaseName != dataBase.Name)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destPath.Context != CremaSchema.TypeDirectory)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destObject is IType)
                    throw new InvalidOperationException($"cannot move to : {destPath}");

                using (DataBaseUsing.Set(dataBase, authentication))
                {
                    if (destObject is ITypeCategory destCategory)
                    {
                        if (sourceCategory.Parent != destCategory)
                            sourceCategory.Move(authentication, destCategory.Path);
                    }
                    else
                    {
                        if (NameValidator.VerifyCategoryPath(destPath.ItemPath) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        var itemName = new ItemName(destPath.ItemPath);
                        var categories = sourceCategory.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                        if (categories.Contains(itemName.CategoryPath) == false)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceCategory.Name != itemName.Name && types.Contains(itemName.Name) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceCategory.Parent.Path != itemName.CategoryPath)
                            sourceCategory.Move(authentication, itemName.CategoryPath);
                        if (sourceCategory.Name != itemName.Name)
                            sourceCategory.Rename(authentication, itemName.Name);
                    }
                }
            }
        }

        private void MoveType(Authentication authentication, IType sourceType, string newPath)
        {
            var destPath = new DataBasePath(newPath);
            var destObject = this.GetObject(authentication, destPath.Path);
            this.CremaHost.Dispatcher.Invoke(MoveType);

            void MoveType()
            {
                var dataBase = sourceType.GetService(typeof(IDataBase)) as IDataBase;
                var types = sourceType.GetService(typeof(ITypeCollection)) as ITypeCollection;

                if (destPath.DataBaseName != dataBase.Name)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destPath.Context != CremaSchema.TypeDirectory)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destObject is IType)
                    throw new InvalidOperationException($"cannot move to : {destPath}");

                using (DataBaseUsing.Set(dataBase, authentication))
                {
                    if (destObject is ITypeCategory destCategory)
                    {
                        if (sourceType.Category != destCategory)
                            sourceType.Move(authentication, destCategory.Path);
                    }
                    else
                    {
                        if (NameValidator.VerifyCategoryPath(destPath.ItemPath) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        var itemName = new ItemName(destPath.ItemPath);
                        var categories = sourceType.GetService(typeof(ITypeCategoryCollection)) as ITypeCategoryCollection;
                        if (categories.Contains(itemName.CategoryPath) == false)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceType.Name != itemName.Name && types.Contains(itemName.Name) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceType.Category.Path != itemName.CategoryPath)
                            sourceType.Move(authentication, itemName.CategoryPath);
                        if (sourceType.Name != itemName.Name)
                            sourceType.Rename(authentication, itemName.Name);
                    }
                }
            };
        }

        private void MoveTableCategory(Authentication authentication, ITableCategory sourceCategory, string newPath)
        {
            var destPath = new DataBasePath(newPath);
            var destObject = this.GetObject(authentication, destPath.Path);
            this.CremaHost.Dispatcher.Invoke(MoveTableCategory);

            void MoveTableCategory()
            {
                var dataBase = sourceCategory.GetService(typeof(IDataBase)) as IDataBase;
                var tables = sourceCategory.GetService(typeof(ITableCollection)) as ITableCollection;

                if (destPath.DataBaseName != dataBase.Name)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destPath.Context != CremaSchema.TableDirectory)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destObject is ITable)
                    throw new InvalidOperationException($"cannot move to : {destPath}");

                using (DataBaseUsing.Set(dataBase, authentication))
                {
                    if (destObject is ITableCategory destCategory)
                    {
                        if (sourceCategory.Parent != destCategory)
                            sourceCategory.Move(authentication, destCategory.Path);
                    }
                    else
                    {
                        if (NameValidator.VerifyCategoryPath(destPath.ItemPath) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        var itemName = new ItemName(destPath.ItemPath);
                        var categories = sourceCategory.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                        if (categories.Contains(itemName.CategoryPath) == false)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceCategory.Name != itemName.Name && tables.Contains(itemName.Name) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceCategory.Parent.Path != itemName.CategoryPath)
                            sourceCategory.Move(authentication, itemName.CategoryPath);
                        if (sourceCategory.Name != itemName.Name)
                            sourceCategory.Rename(authentication, itemName.Name);
                    }
                }
            }
        }

        private void MoveTable(Authentication authentication, ITable sourceTable, string newPath)
        {
            var destPath = new DataBasePath(newPath);
            var destObject = this.GetObject(authentication, destPath.Path);
            this.CremaHost.Dispatcher.Invoke(MoveTable);

            void MoveTable()
            {
                var dataBase = sourceTable.GetService(typeof(IDataBase)) as IDataBase;
                var tables = sourceTable.GetService(typeof(ITableCollection)) as ITableCollection;

                if (destPath.DataBaseName != dataBase.Name)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destPath.Context != CremaSchema.TableDirectory)
                    throw new InvalidOperationException($"cannot move to : {destPath}");
                if (destObject is ITable)
                    throw new InvalidOperationException($"cannot move to : {destPath}");

                using (DataBaseUsing.Set(dataBase, authentication))
                {
                    if (destObject is ITableCategory destCategory)
                    {
                        if (sourceTable.Category != destCategory)
                            sourceTable.Move(authentication, destCategory.Path);
                    }
                    else
                    {
                        if (NameValidator.VerifyCategoryPath(destPath.ItemPath) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        var itemName = new ItemName(destPath.ItemPath);
                        var categories = sourceTable.GetService(typeof(ITableCategoryCollection)) as ITableCategoryCollection;
                        if (categories.Contains(itemName.CategoryPath) == false)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceTable.Name != itemName.Name && tables.Contains(itemName.Name) == true)
                            throw new InvalidOperationException($"cannot move to : {destPath}");
                        if (sourceTable.Category.Path != itemName.CategoryPath)
                            sourceTable.Move(authentication, itemName.CategoryPath);
                        if (sourceTable.Name != itemName.Name)
                            sourceTable.Rename(authentication, itemName.Name);
                    }
                }
            };
        }

        private void MoveDataBase(Authentication authentication, IDataBase dataBase, string newPath)
        {
            this.CremaHost.Dispatcher.Invoke(MoveDataBase);

            void MoveDataBase()
            {
                if (NameValidator.VerifyCategoryPath(newPath) == true)
                    throw new InvalidOperationException($"cannot move {dataBase} to : {newPath}");
                var itemName = new ItemName(newPath);
                if (itemName.CategoryPath != PathUtility.Separator)
                    throw new InvalidOperationException($"cannot move {dataBase} to : {newPath}");

                dataBase.Rename(authentication, itemName.Name);
            }
        }

        public override object GetObject(Authentication authentication, string path)
        {
            return this.CremaHost.Dispatcher.Invoke(GetObject);

            object GetObject()
            {
                var dataBasePath = new DataBasePath(path);

                if (dataBasePath.DataBaseName == string.Empty)
                    return null;

                var dataBase = this.CremaHost.DataBases[dataBasePath.DataBaseName];
                if (dataBase == null)
                    throw new DataBaseNotFoundException(dataBasePath.DataBaseName);

                if (dataBasePath.Context == string.Empty)
                    return dataBase;

                if (dataBasePath.ItemPath == string.Empty)
                    return null;

                if (dataBase.IsLoaded == false)
                    dataBase.Load(authentication);

                if (dataBasePath.Context == CremaSchema.TableDirectory)
                {
                    if (NameValidator.VerifyCategoryPath(dataBasePath.ItemPath) == true)
                        return dataBase.TableContext[dataBasePath.ItemPath];
                    var item = dataBase.TableContext[dataBasePath.ItemPath + PathUtility.Separator];
                    if (item != null)
                        return item;
                    return dataBase.TableContext[dataBasePath.ItemPath];
                }
                else if (dataBasePath.Context == CremaSchema.TypeDirectory)
                    {
                    if (NameValidator.VerifyCategoryPath(dataBasePath.ItemPath) == true)
                        return dataBase.TypeContext[dataBasePath.ItemPath];
                    var item = dataBase.TypeContext[dataBasePath.ItemPath + PathUtility.Separator];
                    if (item != null)
                        return item;
                    return dataBase.TypeContext[dataBasePath.ItemPath];
                }
                else
                {
                    return null;
                }
            }
        }

        private void DataBase_Unloaded(object sender, EventArgs e)
        {
            if (this.CommandContext.IsOnline == true)
                this.CommandContext.Path = PathUtility.Separator;
        }

        private ICremaHost CremaHost => this.cremaHost.Value;

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.CremaHost.Closed += (s, e) => this.dataBasePath = null;
        } 

        #endregion
    }
}
