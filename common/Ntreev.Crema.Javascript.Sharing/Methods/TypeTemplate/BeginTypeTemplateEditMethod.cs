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
    class BeginTypeTemplateEditMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginTypeTemplateEditMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginTypeTemplateEdit);
        }

        [ReturnParameterName("domainID")]
        private string BeginTypeTemplateEdit(string dataBaseName, string typeName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                var type = dataBase.TypeContext.Types[typeName];
                if (type == null)
                    throw new TypeNotFoundException(typeName);
                var authentication = this.Context.GetAuthentication(this);
                type.Template.BeginEdit(authentication);
                return $"{type.Template.Domain.ID}";
            });
        }
    }
}
