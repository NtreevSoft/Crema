using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableContent
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableContent))]
    class BeginTableContentEditMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginTableContentEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginTableContentEdit);
        }

        [ReturnParameterName("domainID")]
        private string BeginTableContentEdit(string dataBaseName, string tableName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                if (table == null)
                    throw new TableNotFoundException(tableName);
                var authentication = this.Context.GetAuthentication(this);
                table.Content.BeginEdit(authentication);
                return $"{table.Content.Domain.ID}";
            });
        }
    }
}
