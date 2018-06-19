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

namespace Ntreev.Crema.Services.Data
{
    class DataBaseSet
    {
        private readonly DataBase dataBase;
        private readonly Dictionary<CremaDataType, Type> types = new Dictionary<CremaDataType, Type>();
        //private readonly Dictionary<CremaDataTable, Table> tables = new Dictionary<CremaDataTable, Table>();

        public DataBaseSet(DataBase dataBase, CremaDataSet dataSet)
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
                this.Tables.Add(item, table);
            }
            this.Validate(dataSet, dataBase);
        }

        public void Validate(CremaDataSet dataSet, DataBase dataBase)
        {
            foreach (var item in dataSet.Types)
            {
                var type = dataBase.TypeContext.Types[item.Name];
                if (type == null)
                    throw new ItemNotFoundException(item.Name);
                if (type.Name == item.Name)
                {
                    if (type.Category.Path != item.CategoryPath)
                        throw new InvalidOperationException(string.Format(Resources.Exception_ItemPathChanged_Format, item.Name, type.Category.Path, item.CategoryPath));
                }
            }
        }

        public void SetTypeCategoryPath(string categoryPath, string newCategoryPath)
        {
            foreach (var item in this.types)
            {
                var dataType = item.Key;
                var type = item.Value;
                if (type.Path.StartsWith(categoryPath) == false)
                    continue;

                dataType.CategoryPath = Regex.Replace(dataType.CategoryPath, "^" + categoryPath, newCategoryPath);
            }
        }

        public void SetTableCategoryPath(TableCategory category, string newCategoryPath)
        {
            foreach (var item in this.Tables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table.Path.StartsWith(category.Path) == false)
                    continue;
                if (table.Parent != null)
                    continue;

                dataTable.CategoryPath = Regex.Replace(dataTable.CategoryPath, "^" + category.Path, newCategoryPath);
            }

            this.SerializeTables();

            var itemPath1 = category.LocalPath;
            var itemPath2 = this.dataBase.TableContext.GenerateCategoryPath(newCategoryPath);
            this.Repository.Move(itemPath1, itemPath2);
        }

        public static void CreateTable(CremaDataSet dataSet, DataBase dataBase)
        {
            var dataBaseSet = new DataBaseSet(dataBase, dataSet);
            dataBaseSet.SerializeTables();
            dataBaseSet.AddRepositoryPath();
        }

        public static void RenameTable(CremaDataSet dataSet, Table table, string tableName)
        {
            var dataBaseSet = new DataBaseSet(table.DataBase, dataSet);
            var dataTable = dataBaseSet.Tables.First(item => item.Value == table).Key;
            dataTable.TableName = tableName;
            dataBaseSet.SerializeTables();
            dataBaseSet.MoveRepositoryPath();
        }

        public void MoveTable(Table table, string categoryPath)
        {
            var dataTable = this.Tables.First(item => item.Value == table).Key;
            dataTable.CategoryPath = categoryPath;

            this.SerializeTables();
            this.MoveRepositoryPath();
        }

        public void SetTableTags(Table table, TagInfo tags)
        {
            var dataTable = this.Tables.First(item => item.Value == table).Key;
            dataTable.Tags = tags;

            this.SerializeTables();
        }

        public void SetTableComment(Table table, string comment)
        {
            var dataTable = this.Tables.First(item => item.Value == table).Key;
            dataTable.Comment = comment;

            this.SerializeTables();
        }

        public void DeleteTable(Table table)
        {
            var dataTable = this.Tables.First(item => item.Value == table).Key;
            var dataSet = dataTable.DataSet;
            dataSet.Tables.Remove(dataTable);
            this.DeleteRepositoryPath();
        }

        public void SerializeTypes()
        {
            foreach (var item in this.types)
            {
                var dataType = item.Key;
                var type = item.Value;
                this.Serializer.Serialize(type.LocalPath, dataType, null);
            }
        }

        public void SerializeTables()
        {
            var context = this.dataBase.TableContext;
            foreach (var item in this.Tables)
            {
                var dataTable = item.Key;
                var table = item.Value;

                var categoryPath = table != null ? table.Category.Path : dataTable.CategoryPath;
                var name = table != null ? table.Name : dataTable.Name;

                var itemPath = context.GenerateTablePath(categoryPath, name);
                var props = new CremaDataTableSerializerSettings(itemPath, table?.TemplatedParent?.LocalPath);
                this.Serializer.Serialize(itemPath, dataTable, props);
            }
        }

        public Dictionary<CremaDataTable, Table> Tables { get; } = new Dictionary<CremaDataTable, Table>();

        public IObjectSerializer Serializer => this.dataBase.Serializer;

        private void AddRepositoryPath()
        {
            foreach (var item in this.Tables)
            {
                var dataTable = item.Key;
                var table = item.Value;
                if (table != null)
                    continue;

                this.AddRepositoryPath(dataTable);
            }
        }

        private void MoveRepositoryPath()
        {
            foreach (var item in this.Tables)
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

        private void DeleteRepositoryPath()
        {
            foreach (var item in this.Tables)
            {
                var dataTable = item.Key;
                var table = item.Value;

                if (dataTable.DataSet != null)
                    continue;

                this.DeleteRepositoryPath(dataTable, table);
            }
        }

        private void AddRepositoryPath(CremaDataTable dataTable)
        {
            var context = this.dataBase.TableContext;
            var directoryName = context.GenerateCategoryPath(dataTable.CategoryPath);
            var files = Directory.GetFiles(directoryName, $"{dataTable.Name}.*").Where(item => Path.GetFileNameWithoutExtension(item) == dataTable.Name).ToArray();
            var status = this.Repository.Status(files);

            foreach (var item in status)
            {
                int qwer = 0;
            }

            //for (var i = 0; i < files.Length; i++)
            //{
            //    var path1 = files[i];
            //    var extension = Path.GetExtension(path1);
            //    var path2 = this.dataBase.TableContext.GeneratePath(dataTable.CategoryPath + dataTable.Name) + extension;
            //    this.Repository.Move(path1, path2);
            //}
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

        
    }
}
