using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableContent
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableContent))]
    class AddTableContentRowMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public AddTableContentRowMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, IDictionary<string, object>, object[]>(this.AddTableContentRow);
        }

        [ReturnParameterName("keys")]
        private object[] AddTableContentRow(string domainID, IDictionary<string, object> fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return content.Dispatcher.Invoke(() =>
            {
                var tableInfo = content.Table.TableInfo;
                var row = content.AddNew(authentication, null);
                foreach (var item in fields)
                {
                    row.SetField(authentication, item.Key, item.Value);
                }
                content.EndNew(authentication, row);
                return tableInfo.Columns.Select(item => row[item.Name]).ToArray();
            });
        }
    }
}
