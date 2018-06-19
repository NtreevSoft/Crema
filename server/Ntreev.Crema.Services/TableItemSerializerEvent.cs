using Ntreev.Crema.Data;
using Ntreev.Crema.Services.Data;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    [Export(typeof(IObjectSerializerEvent))]
    class TableItemSerializerEvent : IObjectSerializerEvent
    {
        public string Name => nameof(ITableItem);

        public bool CanSupport(System.Type type)
        {
            return typeof(ITableItem).IsAssignableFrom(type);
        }

        public string[] Create(IObjectSerializer serializer, string itemPath, object data)
        {
            throw new NotImplementedException();
        }

        public void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        public void Move(object obj, string itemPath, string newItemPath)
        {
            throw new NotImplementedException();
        }

        public object Rename(IObjectSerializer serializer, object obj, string itemPath, string newItemPath)
        {
            if (obj is TableCategory category)
            {
                var dataSet = this.ReadAllData(serializer, Authentication.System, category, itemPath);
                var dataBase = category.DataBase;
                var dataTables = new DataTableCollection(dataSet, dataBase);
                dataTables.SetCategoryPath(category.Path, newItemPath);
                dataTables.Modify(serializer);
                return dataSet;
            }

            throw new NotImplementedException();
        }

        public CremaDataSet ReadAllData(IObjectSerializer serializer, Authentication authentication, TableCategory category, string itemPath)
        {
            var tables = EnumerableUtility.Descendants<IItem, Table>(category as IItem, item => item.Childs).ToArray();
            var typePaths = tables.SelectMany(item => item.GetTypes())
                                  .Select(item => item.LocalPath)
                                  .Distinct()
                                  .ToArray();
            var tablePaths = tables.SelectMany(item => EnumerableUtility.Friends(item, item.DerivedTables))
                                   .Select(item => item.LocalPath)
                                   .Distinct()
                                   .ToArray();

            var props = new CremaDataSetSerializerSettings(authentication, typePaths, tablePaths);
            var dataSet = serializer.Deserialize(itemPath, typeof(CremaDataSet), props) as CremaDataSet;
            return dataSet;
        }
    }
}
