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
    class BeginTableCreateMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginTableCreateMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginTableCreate);
        }

        [ReturnParameterName("domainID")]
        private string BeginTableCreate(string dataBaseName, string categoryPath)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var category = dataBase.TableContext.Categories[categoryPath];
                if (category == null)
                    throw new CategoryNotFoundException(categoryPath);
                var authentication = this.Context.GetAuthentication(this);
                var template = category.NewTable(authentication);
                return $"{template.Domain.ID}";
            });
        }
    }
}
