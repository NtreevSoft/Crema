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
    class DeleteTableTemplateColumnMethod : DomainScriptMethodBase
    {
        [ImportingConstructor]
        public DeleteTableTemplateColumnMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string>(this.DeleteTableTemplateColumn);
        }

        private void DeleteTableTemplateColumn(string domainID, string columnName)
        {
            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var member = template[columnName];
                if (member == null)
                    throw new ItemNotFoundException(columnName);
                member.Delete(authentication);
            });
        }
    }
}
