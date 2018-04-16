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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Diagnostics;
using Ntreev.Crema.ServiceModel;
using System.IO;
using Ntreev.Crema.ApplicationHost.ViewModels;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Services;
using System.Globalization;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System.Windows.Threading;
using System.Text;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ApplicationHost
{
    public class AppBootstrapper : AppBootstrapper<IShell>
    {
        private readonly AppSettings settings = new AppSettings();

        public AppBootstrapper()
        {
            AppMessageBox.MessageSelector = MessageSelector;
        }

        public AppSettings Settings
        {
            get { return this.settings; }
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (this.settings.Culture != string.Empty)
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(this.settings.Culture);
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(this.settings.Culture);
            }

            base.OnStartup(sender, e);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }

        protected override IEnumerable<string> SelectPath()
        {
            var pluginsPath = AppUtility.GetDocumentPath("plugins");

            if (this.settings.PluginsPath != null)
            {
                foreach (var item in this.settings.PluginsPath)
                {
                    yield return item;
                }
            }

            if (Directory.Exists(pluginsPath) == true)
            {
                foreach (var item in Directory.GetDirectories(pluginsPath))
                {
                    yield return item;
                }
            }
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
            this.PublishException(e);
            e.Handled = true;
            if (Application.Current != null)
                Application.Current.Shutdown(-1);
            else
                Environment.Exit(-1);
        }

        protected override bool AutoInitialize
        {
            get { return false; }
        }

        private void PublishException(DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                var publishers = this.GetService(typeof(IEnumerable<IExceptionPublisher>)) as IEnumerable<IExceptionPublisher>;
                foreach (var item in publishers)
                {
                    if (e.Exception == null)
                        item.Publish(null);
                    else
                        item.Publish(e.Exception);

                }
            }
            catch (Exception)
            {
            }

            //var sb = new StringBuilder();

            //sb.AppendLine($"General Information");
            //if (this.settings.ReportDetails == true)
            //    sb.AppendLine($"{nameof(Environment.MachineName)} : {Environment.MachineName}");
            //sb.AppendLine($"DateTime : {DateTime.Now}");
            //sb.AppendLine($"AppDomainName : {AppDomain.CurrentDomain.FriendlyName}");
            //if (this.settings.ReportDetails == true)
            //    sb.AppendLine($"{nameof(Environment.UserDomainName)} : {Environment.UserDomainName}");
            //if (this.settings.ReportDetails == true)
            //    sb.AppendLine($"{nameof(Environment.UserName)} : {Environment.UserName}");
            //sb.AppendLine($"Program Version : {AppUtility.ProductVersion}");
            //sb.AppendLine();

            //sb.AppendLine($"Exception Information");
            //if (e.Exception == null)
            //    sb.AppendLine($"exception object is null");
            //else
            //    sb.AppendLine(e.Exception.ToString());

            //var errorPath = AppUtility.GetDocumentFilename($"errors\\{DateTime.Now.ToString("yyyy-MM-dd_HH_mm")}.log");
            //FileUtility.WriteAllText(sb.ToString(), errorPath);

            //var exceptionWindow = new ExceptionWindow(e.Exception, sb.ToString())
            //{
            //    Topmost = true,
            //    ReportDetails = this.settings.ReportDetails,
            //};
            //if (exceptionWindow != Application.Current.MainWindow)
            //{
            //    exceptionWindow.Owner = Application.Current.MainWindow;
            //}

            //exceptionWindow.ShowDialog();
        }

        private static string MessageSelector(object message)
        {
            if (message is Exception exception)
            {
                CremaLog.Error(exception);
            }
            return AppMessageBox.DefaultMessageSelector(message);
        }
    }
}
