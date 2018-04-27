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
    class GetTableTemplateDomainMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableTemplateDomainMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.GetTableTemplateDomain);
        }

        [ReturnParameterName("domainID")]
        private string GetTableTemplateDomain(string dataBaseName, string tableName)
        {
            var table = this.GetTable(dataBaseName, tableName);
            return table.Dispatcher.Invoke(() =>
            {
                if (table.Template.Domain != null)
                    return $"{table.Template.Domain.ID}";
                return null;
            });
        }
    }
}
