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

using FirstFloor.ModernUI.Windows.Controls;
using Ntreev.Crema.Client.Framework;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ntreev.Crema.ApplicationHost
{
    /// <summary>
    /// ExceptionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExceptionWindow : ModernWindow
    {
        private readonly Exception exception;
        private readonly string exceptionContext;
        private bool isSendMail;

        public ExceptionWindow(Exception e, string exceptionContext)
        {
            InitializeComponent();
            this.exception = e;
            this.exceptionContext = exceptionContext;
            this.ExceptionMessage.Text = exceptionContext;
        }

        public bool ReportDetails
        {
            get; set;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.SendButton.Visibility != Visibility.Visible && this.isSendMail == false)
                e.Cancel = true;

            base.OnClosing(e);
        }

        private async void SendMail(string exceptionMessage)
        {
            try
            {
                var userName = this.ReportDetails == true ? Environment.UserName : "Unknown";
                var mailTo = new MailAddress("cremaqa@gmail.com");
                var mailFrom = new MailAddress("error@ntreev.com", userName);
                var smtpClient = new SmtpClient("smtp.gmail.com", 25);

                using (var mailMessage = new MailMessage(mailFrom, mailTo))
                {
                    mailMessage.Subject = "[Exception] Crema Report - " + userName;
                    mailMessage.Body = exceptionMessage;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("cremaqa", "ehadlmwmboqyditc");
                    await Task.Run(() => smtpClient.Send(mailMessage));
                }

                this.isSendMail = true;

                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
                this.ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                this.Message.Content = Properties.Resources.Message_ReportedException;
            }
            catch (Exception ex)
            {
                AppMessageBox.Show(ex.Message);
            }
            finally
            {
                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.Message.Content = Ntreev.Crema.ApplicationHost.Properties.Resources.Message_ReportingException;
            this.ExceptionMessage.Visibility = Visibility.Collapsed;
            this.SendButton.Visibility = Visibility.Collapsed;
            this.ProgressBar.Visibility = Visibility.Visible;
            this.Width = 460;
            this.SendMail(this.ExceptionMessage.Text);
        }
    }
}
