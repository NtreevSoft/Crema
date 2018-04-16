using Ntreev.ModernUI.Framework.Controls;
using Ntreev.ModernUI.Framework.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.Crema.ApplicationHost
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            this.SendMail(e.Exception);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            this.SendMail(e.Exception);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            this.SendMail(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject == null)
                this.SendMail(new Exception("CurrentDomain_UnhandledException : ExceptionObject is null."));
            else
                this.SendMail(new Exception(e.ExceptionObject.ToString()));
        }

        private void SendMail(Exception e)
        {
            var window = new ExceptionWindow(e)
            {
                Title = "Crema Server App",
                MailAddress = "cremaqa@gmail.com",
                UserName = "cremaqa",
                Password = "ehadlmwmboqyditc",
            };
            window.ShowDialog();
            Environment.Exit(-1);
        }
    }
}
