using System;
using System.Collections.Generic;
using Caliburn.Micro;
using System.Reflection;
using System.Linq;
using Ntreev.ModernUI.Framework;
using FirstFloor.ModernUI.Presentation;
using System.ComponentModel;
using Ntreev.Crema.ApplicationHost.Properties;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Ntreev.Crema.Services;
using System.Configuration;

namespace Ntreev.Crema.ApplicationHost
{
    public class AppBootstrapper : AppBootstrapper<IShell>
    {
        public AppBootstrapper()
        {

        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            var shell = this.GetInstance(typeof(IShell), null) as IShell;
            var configs = this.GetInstance(typeof(IAppConfiguration), null) as IAppConfiguration;

            base.OnStartup(sender, e);
            AppearanceManager.Current.AccentColor = System.Windows.Media.Color.FromRgb(230, 174, 61);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }

        protected override IEnumerable<string> SelectPath()
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var rootPath = Path.GetDirectoryName(dllPath);
            var dirs = Directory.GetDirectories(Path.Combine(rootPath, CremaBootstrapper.RepositoryModulesPath));
            foreach (var item in dirs)
            {
                yield return item;
            }
        }
    }
}
