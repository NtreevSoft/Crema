//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System.Text;
using System.Resources;
using Ntreev.Crema.ApplicationHost.Properties;

namespace Ntreev.Crema.ApplicationHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static LogWriter writer = new LogWriter();

        static App()
        {
            CremaLog.RedirectionWriter = writer;

            RegisterLicense($"{typeof(App).Namespace}.Licenses.Newtonsoft.Json.Schema.license", item => Newtonsoft.Json.Schema.License.RegisterLicense(item));
            RegisterLicense($"{typeof(App).Namespace}.Licenses.Xceed.Wpf.DataGrid.license", item => Xceed.Wpf.DataGrid.Licenser.LicenseKey = item);
            RegisterLicense($"{typeof(App).Namespace}.Licenses.Xceed.Wpf.Toolkit.license", item => Xceed.Wpf.Toolkit.Licenser.LicenseKey = item);

            void RegisterLicense(string licenseName, Action<string> action)
            {
                try
                {
                    using (var stream = typeof(App).Assembly.GetManifestResourceStream(licenseName))
                    using (var reader = new StreamReader(stream))
                    {
                        var license = reader.ReadToEnd();
                        if (license == string.Empty)
                            CremaLog.Warn($"license does not registered : {licenseName}");
                        else
                            action(license);
                    }
                }
                catch (Exception e)
                {
                    CremaLog.Error(new Exception($"register license failed : {licenseName}", e));
                }
            }
        }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                if (this.ParseCommandLine() == false)
                {
                    this.Shutdown();
                    return;
                }

                var splash = new Views.SplashWindow()
                {
                    Title = AppUtility.ProductName,
                    ThemeColor = Settings.Default.ThemeColor,
                    Background = Settings.Default.Background,
                    Foreground = Settings.Default.Foreground
                };
                splash.Show();

                if (this.FindResource("bootstrapper") is AppBootstrapper bootstrapper)
                {
                    bootstrapper.Initialize();
                }
                base.OnStartup(e);
                splash.Close();
            }
            catch (ConfigurationErrorsException cee)
            {
                foreach (var exception in cee.Errors)
                {
                    CremaLog.Error(exception);
                }

                Settings.Default.Reset();

                MessageBox.Show($"설정 파일을 초기화 하였습니다.\r\n응용 프로그램을 다시 시작하십시오.\r\n\r\n{cee}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CremaLog.RedirectionWriter = null;
            base.OnExit(e);
        }

        private bool ParseCommandLine()
        {
            try
            {
                if (this.FindResource("bootstrapper") is AppBootstrapper bootstrapper)
                {
                    var sb = new StringBuilder();
                    var parser = new CommandLineParser(bootstrapper.Settings)
                    {
                        Out = new StringWriter(sb)
                    };
                    if (parser.Parse(Environment.CommandLine) == false)
                    {
                        MessageBox.Show(sb.ToString(), "Usage", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        internal static LogWriter Writer
        {
            get { return writer; }
        }

        #region classes

        class DebugTraceListener : TraceListener
        {
            public override void Write(string message)
            {
            }

            public override void WriteLine(string message)
            {
                Debugger.Break();
            }
        }

        #endregion
    }
}
