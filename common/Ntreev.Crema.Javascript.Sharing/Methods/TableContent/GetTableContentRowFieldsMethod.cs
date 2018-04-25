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
    class GetTableContentRowFieldsMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableContentRowFieldsMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, object[], IDictionary<string, object>>(this.GetTableContentRowFields);
        }

        private IDictionary<string, object> GetTableContentRowFields(string domainID, object[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return content.Dispatcher.Invoke(() =>
            {
                var row = content.Find(authentication, keys);
                var tableInfo = content.Table.TableInfo;
                var fields = new Dictionary<string, object>(tableInfo.Columns.Length);
                foreach (var item in tableInfo.Columns)
                {
                    fields.Add(item.Name, row[item.Name]);
                }
                return fields;
            });
        }
    }
}
