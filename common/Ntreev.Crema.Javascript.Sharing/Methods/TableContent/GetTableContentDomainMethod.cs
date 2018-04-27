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
    class GetTableContentDomainMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public GetTableContentDomainMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.GetTableContentDomain);
        }

        [ReturnParameterName("domainID")]
        private string GetTableContentDomain(string dataBaseName, string tableName)
        {
            var table = this.GetTable(dataBaseName, tableName);
            return table.Dispatcher.Invoke(() =>
            {
                if (table.Content.Domain != null)
                    return $"{table.Content.Domain.ID}";
                return null;
            });
        }
    }
}
