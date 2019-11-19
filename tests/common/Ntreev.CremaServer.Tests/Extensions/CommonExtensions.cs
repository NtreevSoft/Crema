using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.CremaServer.Tests.Extensions
{
    public static class CommonExtensions
    {
        public static Authentication GetAdminAuthentication(this ICremaHost cremaHost)
        {
            return cremaHost.Dispatcher.Invoke(() => cremaHost.Login("admin", "admin".ToSecureString()));
        }
    }
}
