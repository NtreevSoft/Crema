using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class BeginTypeCreateMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginTypeCreateMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginTypeCreate);
        }

        [ReturnParameterName("domainID")]
        private string BeginTypeCreate(string dataBaseName, string categoryPath)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var category = dataBase.TypeContext.Categories[categoryPath];
                var authentication = this.Context.GetAuthentication(this);
                var template = category.NewType(authentication);
                return $"{template.Domain.ID}";
            });
        }
    }
}
