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
    class SetTableContentRowFieldMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTableContentRowFieldMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, object[], string, object>(this.SetTableContentRowField);
        }

        private void SetTableContentRowField(string domainID, object[] keys, string columnName, object value)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            content.Dispatcher.Invoke(() =>
            {
                var row = content.Find(authentication, keys);
                row.SetField(authentication, columnName, value);
            });
        }
    }
}
