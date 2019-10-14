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
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;
using Ntreev.Crema.Client.Framework.Controls.Notifications;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Ntreev.Crema.Client.Framework
{
    [Export(typeof(IToastMessageService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ToastMessageService : IToastMessageService
    {
        private readonly ICremaAppHost cremaAppHost;
        private Notifier notifier;

        [ImportingConstructor]
        public ToastMessageService(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Closed += CremaAppHostOnClosed;
        }

        public void Show(string message, string title)
        {
            this.notifier = this.notifier ?? CreateNotifier();
            this.notifier.Notify(() => new NotifyMessage(message, title, CreateMessageOptions()));
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(config =>
            {
                config.PositionProvider = new WindowPositionProvider(Application.Current.MainWindow, Corner.BottomRight, 5, 5);
                config.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(10), MaximumNotificationCount.UnlimitedNotifications());
                config.DisplayOptions.TopMost = false;
                config.DisplayOptions.Width = 350;
                config.Dispatcher = Dispatcher.CurrentDispatcher;
            });
        }

        private MessageOptions CreateMessageOptions()
        {
            return new MessageOptions
            {
                FreezeOnMouseEnter = true,
                ShowCloseButton = true
            };
        }

        private void CremaAppHostOnClosed(object sender, EventArgs e)
        {
            this.notifier?.Dispose();
            this.notifier = null;
        }
    }
}