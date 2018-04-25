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
    class SetTableContentRowFieldsMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public SetTableContentRowFieldsMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, object[], IDictionary<string, object>>(this.SetTableContentRowFields);
        }

        private void SetTableContentRowFields(string domainID, object[] keys, IDictionary<string, object> fields)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            content.Dispatcher.Invoke(() =>
            {
                var row = content.Find(authentication, keys);
                foreach (var item in fields)
                {
                    row.SetField(authentication, item.Key, item.Value);
                }
            });
        }
    }
}
