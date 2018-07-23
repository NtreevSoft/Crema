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
        }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (this.ParseCommandLine() == false)
            {
                this.Shutdown();
                return;
            }

            var splash = new Views.SplashWindow()
            {
                Title = AppUtility.ProductName,
                ThemeColor = Ntreev.Crema.ApplicationHost.Properties.Settings.Default.ThemeColor,
                Background = Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Background,
                Foreground = Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Foreground
            };
            splash.Show();

            if (this.FindResource("bootstrapper") is AppBootstrapper bootstrapper)
            {
                bootstrapper.Initialize();
            }
            base.OnStartup(e);
            splash.Close();
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
