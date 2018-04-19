using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class AddTypeMemberMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public AddTypeMemberMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, long, string>(this.AddTypeMember);
        }

        private void AddTypeMember(string domainID, string name, long value, string comment)
        {
            if (domainID == null)
                throw new ArgumentNullException(nameof(domainID));

            if (this.CremaHost.GetService(typeof(IDomainContext)) is IDomainContext domainContext)
            {
                if (Guid.TryParse(domainID, out var guid) == true)
                {
                    var domain = domainContext.Dispatcher.Invoke(() => domainContext.Domains[guid]);
                    if (domain.Host is ITypeTemplate template)
                    {
                        var authentication = this.Context.GetAuthentication(this);
                        template.Dispatcher.Invoke(() =>
                        {
                            var typeMember = template.AddNew(authentication);
                            typeMember.SetName(authentication, name);
                            typeMember.SetValue(authentication, value);
                            if (comment != null)
                                typeMember.SetComment(authentication, comment);
                            template.EndNew(authentication, typeMember);
                        });
                        return;
                    }
                }
                throw new ArgumentException("Invalid domainID", nameof(domainID));
            }
            throw new NotImplementedException();
        }
    }
}
