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
    class GetTableContentRowCountMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableContentRowCountMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, object[], string, object>(this.GetTableContentRowCount);
        }

        private object GetTableContentRowCount(string domainID, object[] keys, string columnName)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var content = this.GetDomainHost<ITableContent>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            return content.Dispatcher.Invoke(() => content.Count);
        }
    }
}
