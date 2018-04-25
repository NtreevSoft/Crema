using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TypeTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TypeTemplate))]
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
            if (categoryPath == null)
                throw new ArgumentNullException(nameof(categoryPath));
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var category = dataBase.TypeContext.Categories[categoryPath];
                if (category == null)
                    throw new CategoryNotFoundException(categoryPath);
                var authentication = this.Context.GetAuthentication(this);
                var template = category.NewType(authentication);
                return $"{template.Domain.ID}";
            });
        }
    }
}
