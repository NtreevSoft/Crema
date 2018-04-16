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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Input;
using Ntreev.Crema.Client.Framework;
using System.Reflection;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ntreev.Crema.Services;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.ComponentModel;
using Ntreev.ModernUI.Framework;
using System.Windows.Controls;
using System.Windows.Data;
using Ntreev.Library;
using System.Windows.Threading;
using Ntreev.Library.IO;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.ApplicationHost.Views
{
    [Export(typeof(ShellView))]
    public partial class ShellView : ModernWindow, INotifyPropertyChanged
    {
        public static DependencyProperty ToolsProperty =
            DependencyProperty.Register(nameof(Tools), typeof(ToolBar), typeof(ShellView),
                new FrameworkPropertyMetadata(null));

        public static DependencyProperty IsLogVisibleProperty =
            DependencyProperty.Register(nameof(IsLogVisible), typeof(bool), typeof(ShellView),
                new FrameworkPropertyMetadata(false, IsLogVisiblePropertyChangedCallback));

        public static DependencyProperty LogViewHeightProperty =
            DependencyProperty.Register(nameof(LogViewHeight), typeof(double), typeof(ShellView),
                new FrameworkPropertyMetadata(50.0));

        private readonly IMenuService menuService;
        private readonly ICremaHost cremaHost;
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private IAppConfiguration configs = null;
        [Import]
        private Lazy<IShell> shell = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellView()
        {
            InitializeComponent();
        }

        [ImportingConstructor]
        public ShellView(IMenuService menuService, ICremaHost cremaHost, ICremaAppHost cremaAppHost)
        {
            InitializeComponent();

            this.menuService = menuService;
            this.cremaHost = cremaHost;
            this.cremaAppHost = cremaAppHost;
            foreach (var item in this.menuService.ItemsSource)
            {
                this.SetInputBindings(item);
            }
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closing += CremaHost_Closing;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;

            App.Writer.TextBox = this.logView;
            this.InitializeFromSettings();
        }

        public ToolBar Tools
        {
            get { return (ToolBar)this.GetValue(ToolsProperty); }
            set { this.SetValue(ToolsProperty, value); }
        }

        [ConfigurationProperty("isLogVisible")]
        public bool IsLogVisible
        {
            get { return (bool)this.GetValue(IsLogVisibleProperty); }
            set { this.SetValue(IsLogVisibleProperty, value); }
        }

        [ConfigurationProperty("logViewHeight")]
        public double LogViewHeight
        {
            get { return (double)this.GetValue(LogViewHeightProperty); }
            set { this.SetValue(LogViewHeightProperty, value); }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.Property.Name));
        }

        private void CremaHost_Opened(object sender, EventArgs e)
        {
            var userContext = this.cremaHost.GetService(typeof(IUserContext)) as IUserContext;

            userContext.UsersKicked += UserContext_UsersKicked;
            userContext.UsersBanChanged += UserContext_UsersBanChanged;
            //userContext.MessageReceived += UserContext_MessageReceived;

            var logService = this.cremaHost.GetService(typeof(ILogService)) as ILogService;
            logService.RedirectionWriter = new LogWriter() { TextBox = this.logView, };
        }

        private void CremaHost_Closing(object sender, EventArgs e)
        {
            var logService = this.cremaHost.GetService(typeof(ILogService)) as ILogService;
            if (this.cremaHost.Address != null && logService.RedirectionWriter is LogWriter writer)
            {
                writer.TextBox = null;
            }
            logService.RedirectionWriter = null;
        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            var foreground = this.FindResource("WindowText");
            var background = this.FindResource("WindowBackground");

            if (foreground is SolidColorBrush)
                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Foreground = foreground as SolidColorBrush;
            if (background is SolidColorBrush)
                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Background = background as SolidColorBrush;
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            var i = Application.Current.Windows.Count;
        }

        private void UserContext_UsersKicked(object sender, ItemsEventArgs<IUser> e)
        {
            var userID = this.cremaHost.UserID;
            var userIDs = e.Items.Select(item => item.ID).ToArray();
            var comments = e.MetaData as string[];

            this.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < userIDs.Length; i++)
                {
                    if (userIDs[i] == userID)
                    {
                        if (this.IsActive == false)
                        {
                            FlashWindowUtility.FlashWindow(this);
                        }
                        AppMessageBox.Show(comments[i], Properties.Resources.Message_KickedByAdministrator);
                        break;
                    }
                }
            });
        }

        private async void UserContext_UsersBanChanged(object sender, ItemsEventArgs<IUser> e)
        {
            foreach (var item in e.Items)
            {
                if (item.ID == this.cremaHost.UserID && item.Path != string.Empty)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        if (this.IsActive == false)
                        {
                            FlashWindowUtility.FlashWindow(this);
                        }
                        AppMessageBox.Show(item.BanInfo.Comment, Properties.Resources.Message_BannedByAdministrator);
                    });
                    break;
                }
            }
        }

        //private async void UserContext_MessageReceived(object sender, MessageEventArgs e)
        //{
        //    var userID = this.cremaHost.UserID;
        //    if (e.TargetID == userID || e.UserID == userID)
        //        return;

        //    await this.Dispatcher.InvokeAsync(() =>
        //    {
        //        if (this.IsActive == false)
        //        {
        //            FlashWindowUtility.FlashWindow(this);
        //        }
        //    });
        //}

        private void SetInputBindings(IMenuItem menuItem)
        {
            if (menuItem.InputGesture != null)
            {
                if (menuItem.IsVisible == true)
                    this.InputBindings.Add(new InputBinding(menuItem.Command, menuItem.InputGesture));

                if (menuItem is INotifyPropertyChanged notifyObject)
                {
                    notifyObject.PropertyChanged += (s, e) =>
                    {
                        if (menuItem.IsVisible == true)
                        {
                            this.InputBindings.Add(new InputBinding(menuItem.Command, menuItem.InputGesture));
                        }
                        else
                        {
                            for (var i = 0; i < this.InputBindings.Count; i++)
                            {
                                if (this.InputBindings[i].Command == menuItem)
                                {
                                    this.InputBindings.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    };
                }
            }

            foreach (var item in menuItem.ItemsSource)
            {
                this.SetInputBindings(item);
            }
        }

        private void ModernWindow_Activated(object sender, EventArgs e)
        {
            FlashWindowUtility.StopFlashingWindow(this);
            //FlashWindowUtility.FlashWindow(this);
        }

        private void ModernWindow_Initialized(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.shell.Value.ServiceChanged += Value_ServiceChanged;
            });
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
            this.configs.Update(this);
            if (this.IsLogVisible == true)
                this.logRow.Height = new GridLength(this.LogViewHeight);
            this.shell.Value.Closed += Shell_Closed;
            this.Dispatcher.InvokeAsync(this.ConnectWithSettings, DispatcherPriority.Background);
        }

        private void Value_ServiceChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Shell_Closed(object sender, EventArgs e)
        {
            if (this.IsLogVisible == true)
                this.LogViewHeight = this.logRow.ActualHeight;
            this.configs.Commit(this);
        }

        private void ModernWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void IsLogVisiblePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ShellView;
            var isVisible = (bool)e.NewValue;
            if (isVisible == true)
            {
                self.splitter.Visibility = Visibility.Visible;
                self.logRow.Height = new GridLength(self.LogViewHeight);
            }
            else
            {
                self.splitter.Visibility = Visibility.Collapsed;
                self.LogViewHeight = self.logRow.ActualHeight;
                self.logRow.Height = new GridLength(0, GridUnitType.Auto);
            }
        }

        private void ConnectWithSettings()
        {
            if (this.FindResource("bootstrapper") is AppBootstrapper bootstrapper && Uri.TryCreate(bootstrapper.Settings.Address, UriKind.Absolute, out var uri))
            {
                try
                {
                    var ss = uri.UserInfo.Split(':');
                    var dataBaseName = uri.LocalPath.TrimStart(PathUtility.SeparatorChar);
                    this.cremaAppHost.Login(uri.Authority, ss[0], StringUtility.ToSecureString(ss[1]), dataBaseName);
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                }
            }
        }

        private void InitializeFromSettings()
        {
            if (this.FindResource("bootstrapper") is AppBootstrapper bootstrapper)
            {
                if (bootstrapper.Settings.Theme != string.Empty)
                {
                    this.cremaAppHost.Theme = bootstrapper.Settings.Theme;

                }

                if (bootstrapper.Settings.ThemeColor != string.Empty)
                {
                    try
                    {
                        var themeColor = (Color)ColorConverter.ConvertFromString(bootstrapper.Settings.ThemeColor);
                        this.cremaAppHost.ThemeColor = themeColor;
                    }
                    catch (Exception e)
                    {
                        CremaLog.Error(e);
                    }

                }
            }
        }
    }
}
