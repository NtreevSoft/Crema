using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Services.Data
{
    class DataBaseSet
    {
        private readonly DataBase dataBase;
        private readonly Dictionary<CremaDataType, Type> types = new Dictionary<CremaDataType, Type>();
        private readonly Dictionary<CremaDataTable, Table> tables = new Dictionary<CremaDataTable, Table>();

        private DataBaseSet(DataBase dataBase, CremaDataSet dataSet)
        {
            this.dataBase = dataBase;

            foreach (var item in dataSet.Types)
            {
                var type = dataBase.TypeContext.Types[item.Name, item.CategoryPath];
                this.types.Add(item, type);
            }

            foreach (var item in dataSet.Tables)
            {
                var table = dataBase.TableContext.Tables[item.Name, item.CategoryPath];
                this.tables.Add(item, table);
            }
        }

        public static void SetTypeCategoryPath(CremaDataSet dataSet, TypeCategory category, string newCategoryPath)
        {
            var dataBaseSet = new DataBaseSet(category.DataBase, dataSet);
            foreach (var item in dataBaseSet.types)
            {
                var dataType = item.Key;
                var type = item.Value;
                if (type.Path.StartsWith(category.Path) == false)
                    continue;

                dataType.CategoryPath = Regex.Replace(dataType.CategoryPath, "^" + category.Path, newCategoryPath);
            }

            dataBaseSet.Serialize();

            var itemPath1 = category.LocalPath;
            var itemPath2 = dataBaseSet.dataBase.TypeContext.GenerateCategoryPath(newCategoryPath);
            dataBaseSet.Repository.Move(itemPath1, itemPath2);
        }

        public static void SetTableCategoryPath(CremaDataSet dataSet, TableCategory category, string newCategoryPath)
        {
            var dataBaseSet = new DataBaseSet(category.DataBase, dataSet);
            foreach (var item in dataBaseSet.tables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table.Path.StartsWith(category.Path) == false)
                    continue;
                if (table.Parent != null)
                    continue;

                dataTable.CategoryPath = Regex.Replace(dataTable.CategoryPath, "^" + category.Path, newCategoryPath);
            }

            dataBaseSet.Serialize();

            var itemPath1 = category.LocalPath;
            var itemPath2 = dataBaseSet.dataBase.TableContext.GenerateCategoryPath(newCategoryPath);
            dataBaseSet.Repository.Move(itemPath1, itemPath2);
        }

        public static void CreateType(CremaDataSet dataSet, DataBase dataBase)
        {
            var dataBaseSet = new DataBaseSet(dataBase, dataSet);
            dataBaseSet.Serialize();
            dataBaseSet.AddTypesRepositoryPath();
        }

        public static void RenameType(CremaDataSet dataSet, Type type, string typeName)
        {
            var dataBaseSet = new DataBaseSet(type.DataBase, dataSet);
            var dataType = dataBaseSet.types.First(item => item.Value == type).Key;
            dataType.TypeName = typeName;
            dataBaseSet.Serialize();
            dataBaseSet.MoveTypesRepositoryPath();
        }

        public static void MoveType(CremaDataSet dataSet, Type type, string categoryPath)
        {
            var dataBaseSet = new DataBaseSet(type.DataBase, dataSet);
            var dataType = dataBaseSet.types.First(item => item.Value == type).Key;
            dataType.CategoryPath = categoryPath;
            dataBaseSet.Serialize();
            dataBaseSet.MoveTypesRepositoryPath();
        }

        public static void DeleteType(CremaDataSet dataSet, Type type)
        {
            var dataBaseSet = new DataBaseSet(type.DataBase, dataSet);
            var dataType = dataBaseSet.types.First(item => item.Value == type).Key;
            dataSet.Types.Remove(dataType);
            dataBaseSet.DeleteTypesRepositoryPath();
        }

        public static void ModifyType(CremaDataSet dataSet, Type type)
        {
            var dataBaseSet = new DataBaseSet(type.DataBase, dataSet);
            dataBaseSet.Serialize();
        }

        public static void SetTypeTags(CremaDataSet dataSet, Type type, TagInfo tags)
        {
            var dataBaseSet = new DataBaseSet(type.DataBase, dataSet);
            var dataType = dataBaseSet.types.First(item => item.Value == type).Key;
            dataType.Tags = tags;
            dataBaseSet.Serialize();
        }

        public static void CreateTable(CremaDataSet dataSet, DataBase dataBase)
        {
            var dataBaseSet = new DataBaseSet(dataBase, dataSet);
            dataBaseSet.Serialize();
            dataBaseSet.AddTablesRepositoryPath();
        }

        public static void RenameTable(CremaDataSet dataSet, Table table, string tableName)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.tables.First(item => item.Value == table).Key;
            dataTable.TableName = tableName;
            dataBaseSet.Serialize();
            dataBaseSet.MoveTablesRepositoryPath();
        }

        public static void MoveTable(CremaDataSet dataSet, Table table, string categoryPath)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.tables.First(item => item.Value == table).Key;
            dataTable.CategoryPath = categoryPath;
            dataBaseSet.Serialize();
            dataBaseSet.MoveTablesRepositoryPath();
        }

        public static void SetTableTags(CremaDataSet dataSet, Table table, TagInfo tags)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.tables.First(item => item.Value == table).Key;
            dataTable.Tags = tags;
            dataBaseSet.Serialize();
        }

        public static void SetTableComment(CremaDataSet dataSet, Table table, string comment)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.tables.First(item => item.Value == table).Key;
            dataTable.Comment = comment;
            dataBaseSet.Serialize();
        }

        public static void DeleteTable(CremaDataSet dataSet, Table table)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.tables.First(item => item.Value == table).Key;
            dataSet.Tables.Remove(dataTable);
            dataBaseSet.DeleteTablesRepositoryPath();
        }

        public static void ModifyTable(CremaDataSet dataSet, Table table)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            dataBaseSet.Serialize();
        }

        private void Serialize()
        {
            var typeContext = this.dataBase.TypeContext;
            foreach (var item in this.types)
            {
                var dataType = item.Key;
                var type = item.Value;
                if (type != null)
                {
                    var itemPath = typeContext.GenerateTypePath(type.Category.Path, type.Name);
                    this.Serializer.Serialize(itemPath, dataType, ObjectSerializerSettings.Empty);
                }
                else
                {
                    var itemPath = typeContext.GenerateTypePath(dataType.CategoryPath, dataType.Name);
                    this.Serializer.Serialize(itemPath, dataType, ObjectSerializerSettings.Empty);
                }
            }

            var tableContext = this.dataBase.TableContext;
            foreach (var item in this.tables)
            {
                var dataTable = item.Key;
                var table = item.Value;

                if (table != null)
                {
                    var itemPath = tableContext.GenerateTablePath(table.Category.Path, table.Name);
                    var props = new CremaDataTableSerializerSettings(table.LocalPath, table.TemplatedParent?.LocalPath);
                    this.Serializer.Serialize(itemPath, dataTable, props);
                }
                else
                {
                    var itemPath = tableContext.GenerateTablePath(dataTable.CategoryPath, dataTable.Name);
                    var props = new CremaDataTableSerializerSettings(dataTable.Namespace, dataTable.TemplateNamespace);
                    this.Serializer.Serialize(itemPath, dataTable, props);
                }
            }
        }

        private void AddTypesRepositoryPath()
        {
            foreach (var item in this.types)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table != null)
                    continue;

                this.AddRepositoryPath(dataTable);
            }
        }

        private void MoveTypesRepositoryPath()
        {
            foreach (var item in this.types)
            {
                var dataType = item.Key;
                var type = item.Value;

                var path1 = type.Path;
                var path2 = dataType.CategoryPath + dataType.Name;

                if (path1 == path2)
                    continue;

                this.MoveRepositoryPath(dataType, type);
            }
        }

        private void DeleteTypesRepositoryPath()
        {
            foreach (var item in this.types)
            {
                var dataType = item.Key;
                var type = item.Value;

                if (dataType.DataSet != null)
                    continue;

                this.DeleteRepositoryPath(dataType, type);
            }
        }

        private void AddTablesRepositoryPath()
        {
            foreach (var item in this.tables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table != null)
                    continue;

                this.AddRepositoryPath(dataTable);
            }
        }

        private void MoveTablesRepositoryPath()
        {
            foreach (var item in this.tables)
            {
                var dataTable = item.Key;
                var table = item.Value;

                var path1 = table.Path;
                var path2 = dataTable.CategoryPath + dataTable.Name;

                if (path1 == path2)
                    continue;

                this.MoveRepositoryPath(dataTable, table);
            }
        }

        private void DeleteTablesRepositoryPath()
        {
            foreach (var item in this.tables)
            {
                var dataTable = item.Key;
                var table = item.Value;

                if (dataTable.DataSet != null)
                    continue;

                this.DeleteRepositoryPath(dataTable, table);
            }
        }

        private void AddRepositoryPath(CremaDataType dataType)
        {
            var context = this.dataBase.TypeContext;
            var directoryName = context.GenerateCategoryPath(dataType.CategoryPath);
            var files = Directory.GetFiles(directoryName, $"{dataType.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == dataType.Name).ToArray();
            var status = this.Repository.Status(files);

            foreach (var item in status)
            {
                if (item.Status == RepositoryItemStatus.Untracked)
                {
                    //if (dataType.SourceTable == null)
                    {
                        this.Repository.Add(item.Path);
                    }
                    //else
                    //{
                    //    var extension = Path.GetExtension(item.Path);
                    //    var sourceTable = dataType.SourceTable;
                    //    var sourcePath = context.GenerateTablePath(sourceTable.CategoryPath, sourceTable.Name) + extension;
                    //    FileUtility.Backup(item.Path);
                    //    try
                    //    {
                    //        this.Repository.Copy(sourcePath, item.Path);
                    //    }
                    //    finally
                    //    {
                    //        FileUtility.Restore(item.Path);
                    //    }
                    //}
                }
            }
        }

        private void MoveRepositoryPath(CremaDataType dataType, Type type)
        {
            var directoryName = Path.GetDirectoryName(type.LocalPath);
            var files = Directory.GetFiles(directoryName, $"{type.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == type.Name).ToArray();

            for (var i = 0; i < files.Length; i++)
            {
                var path1 = files[i];
                var extension = Path.GetExtension(path1);
                var path2 = this.dataBase.TypeContext.GeneratePath(dataType.CategoryPath + dataType.Name) + extension;
                this.Repository.Move(path1, path2);
            }
        }

        private void DeleteRepositoryPath(CremaDataType dataType, Type type)
        {
            var directoryName = Path.GetDirectoryName(type.LocalPath);
            var files = Directory.GetFiles(directoryName, $"{type.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == type.Name).ToArray();
            this.Repository.DeleteRange(files);
        }

        private void AddRepositoryPath(CremaDataTable dataTable)
        {
            var context = this.dataBase.TableContext;
            var directoryName = context.GenerateCategoryPath(dataTable.CategoryPath);
            var files = Directory.GetFiles(directoryName, $"{dataTable.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == dataTable.Name).ToArray();
            var status = this.Repository.Status(files);

            foreach (var item in status)
            {
                if (item.Status == RepositoryItemStatus.Untracked)
                {
                    if (dataTable.SourceTable == null)
                    {
                        this.Repository.Add(item.Path);
                    }
                    else
                    {
                        var extension = Path.GetExtension(item.Path);
                        var sourceTable = dataTable.SourceTable;
                        var sourcePath = context.GenerateTablePath(sourceTable.CategoryPath, sourceTable.Name) + extension;
                        FileUtility.Backup(item.Path);
                        try
                        {
                            this.Repository.Copy(sourcePath, item.Path);
                        }
                        finally
                        {
                            FileUtility.Restore(item.Path);
                        }
                    }
                }
            }
        }

        private void MoveRepositoryPath(CremaDataTable dataTable, Table table)
        {
            var directoryName = Path.GetDirectoryName(table.LocalPath);
            var files = Directory.GetFiles(directoryName, $"{table.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == table.Name).ToArray();

            for (var i = 0; i < files.Length; i++)
            {
                var path1 = files[i];
                var extension = Path.GetExtension(path1);
                var path2 = this.dataBase.TableContext.GeneratePath(dataTable.CategoryPath + dataTable.Name) + extension;
                this.Repository.Move(path1, path2);
            }
        }

        private void DeleteRepositoryPath(CremaDataTable dataTable, Table table)
        {
            var directoryName = Path.GetDirectoryName(table.LocalPath);
            var files = Directory.GetFiles(directoryName, $"{table.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == table.Name).ToArray();
            this.Repository.DeleteRange(files);
        }

        private DataBaseRepositoryHost Repository => this.dataBase.Repository;

        private IObjectSerializer Serializer => this.dataBase.Serializer;
    }
}
