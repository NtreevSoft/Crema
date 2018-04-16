using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Random;

namespace Ntreev.Crema.ServerService.Test
{
    class OnlineUserCollection : Dictionary<IUser, Authentication>
    {
        public void Initialize(ICremaHost cremaHost)
        {
            cremaHost.Dispatcher.Invoke(() =>
            {
                var userContext = cremaHost.GetService<IUserContext>();

                foreach (var item in userContext.Users)
                {
                    var authentication = cremaHost.Login(item.ID, item.Authority.ToString().ToLower());
                    this.Add(item, authentication);
                }
            });
        }

        public Authentication RandomAuthentication(Authority authority)
        {
            return this.Where(item => item.Key.Authority == authority).Random().Value;
        }
    }
}
