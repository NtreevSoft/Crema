using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableTemplate))]
    class BeginChildTableCreateMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginChildTableCreateMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginChildTableCreate);
        }

        [ReturnParameterName("domainID")]
        private string BeginChildTableCreate(string dataBaseName, string tableName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                if (table == null)
                    throw new TableNotFoundException(tableName);
                var authentication = this.Context.GetAuthentication(this);
                var template = table.NewTable(authentication);
                return $"{template.Domain.ID}";
            });
        }
    }
}
