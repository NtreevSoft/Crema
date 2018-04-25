using Ntreev.Crema.Data.Xml.Schema;
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
    class ContainsTableTemplateColumnMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public ContainsTableTemplateColumnMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, bool>(this.ContainsTableTemplateColumn);
        }

        private bool ContainsTableTemplateColumn(string domainID, string columName)
        {
            var template = this.GetDomainHost<ITableTemplate>(domainID);
            return template.Dispatcher.Invoke(() => template.Contains(columName));
        }
    }
}
