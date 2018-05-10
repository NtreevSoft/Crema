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
using Ntreev.Library;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using Caliburn.Micro;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Base.Properties;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Base.Dialogs.ViewModels;
using System.ComponentModel.Composition.Hosting;
using System.Collections.ObjectModel;
using System.Collections;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.ComponentModel;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;

namespace Ntreev.Crema.Client.Base.Services.ViewModels
{
    [Export(typeof(ICremaAppHost)), PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    [Export(typeof(IContentService))]
    class CremaAppHostViewModel : ViewModelBase, ICremaAppHost, IPartImportsSatisfiedNotification, IContentService
    {
        private readonly static Dictionary<string, Uri> themes = new Dictionary<string, Uri>(StringComparer.CurrentCultureIgnoreCase);
        private readonly ConnectionItemCollection connectionItems;
        private ConnectionItemViewModel connectionItem;
        private ICommand loginCommand;
        private SecureString securePassword;

        private bool hasError;
        private bool isOpened;
        private bool isLoaded;
        private bool isEncrypted;

        private readonly ICremaHost cremaHost;
        private readonly IAppConfiguration configs;

        private Authenticator authenticator;
        private string dataBaseName;
        private Guid token;
        private Color themeColor;
        private string theme;
        private Authority authority;
        private string address;

        [Import]
        private Lazy<DataBaseServiceViewModel> dataBaseService = null;
        [Import]
        private Lazy<DataBaseListViewModel> dataBaseSelections = null;
        private ICompositionService compositionService;
        private IDataBase dataBase;

        [Import]
        private Lazy<IShell> shell = null;

        private string filterExpression;
        private bool caseSensitive;
        private bool globPattern;

        static CremaAppHostViewModel()
        {
            themes.Add("Dark", new Uri("/Ntreev.Crema.Client.Framework;component/Assets/CremaUI.Dark.xaml", UriKind.Relative));
            themes.Add("Light", new Uri("/Ntreev.Crema.Client.Framework;component/Assets/CremaUI.Light.xaml", UriKind.Relative));
        }

        public static Dictionary<string, Uri> Themes
        {
            get { return themes; }
        }

        [ImportingConstructor]
        public CremaAppHostViewModel(ICremaHost cremaHost, IAppConfiguration configs, ICompositionService compositionService)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.configs = configs;
            this.compositionService = compositionService;
            this.theme = Themes.Keys.FirstOrDefault();
            this.themeColor = FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor;
            this.loginCommand = new DelegateCommand((p) => this.Login(), (p) => this.CanLogin);
            this.connectionItems = ConnectionItemCollection.Read(AppUtility.GetDocumentFilename("ConnectionList.xml"));
            this.compositionService.SatisfyImportsOnce(this.connectionItems);
            this.ConnectionItem = this.connectionItems.FirstOrDefault(item => item.IsDefault);
            this.authenticator = this.cremaHost.GetService(typeof(Authenticator)) as Authenticator;
            this.configs.Update(this);
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(this.IsProgressing))
                {
                    this.Shell.IsProgressing = this.IsProgressing;
                }
                else if (e.PropertyName == nameof(this.ProgressMessage))
                {
                    this.Shell.ProgressMessage = this.ProgressMessage;
                }
            };
        }

        public override string ToString()
        {
            return Resources.Title_Start;
        }

        public void MoveToWiki()
        {
            Process.Start("https://github.com/NtreevSoft/Crema/wiki");
        }

        public void ShowHelp()
        {
            Process.Start("https://github.com/NtreevSoft/Crema/wiki");
        }

        public void Login()
        {
            this.Login(this.ConnectionItem.Address, this.ConnectionItem.ID, this.securePassword, this.ConnectionItem.DataBaseName);
        }

        public async void Login(string address, string userID, SecureString password, string dataBaseName)
        {
            this.HasError = false;
            this.BeginProgress();

            try
            {
                this.ProgressMessage = Resources.Message_ConnectingToServer;
                if (await this.cremaHost.Dispatcher.InvokeAsync(() => CremaBootstrapper.IsOnline(address, userID, password)) == true)
                {
                    if (AppMessageBox.ShowQuestion(Resources.Message_SameIDConnected) == false)
                    {
                        this.ProgressMessage = string.Empty;
                        this.EndProgress();
                        this.HasError = false;
                        return;
                    }
                }
                await this.OpenAsync(address, userID, password);

                if (dataBaseName != string.Empty)
                {
                    await this.LoadAsync(dataBaseName);
                }
                else
                {
                    this.EndProgress();
                    this.dataBase = null;
                    this.dataBaseName = null;
                    this.address = null;
                    this.Refresh();
                    this.ProgressMessage = string.Empty;
                }
            }
            catch (TimeoutException)
            {
                this.ErrorMessage = Resources.Message_ConnectionFailed;
            }
            catch (EndpointNotFoundException)
            {
                this.ErrorMessage = Resources.Message_ConnectionFailed;
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.Message;
                if (this.IsOpened == true)
                {
                    AppMessageBox.ShowError(e.Message);
                }
            }
            finally
            {
                this.EndProgress(this.ProgressMessage);
            }
        }

        public async void Logout()
        {
            this.ErrorMessage = string.Empty;
            if (this.DataBaseName != string.Empty)
                await this.UnloadAsync();

            await this.CloseAsync();
            this.NotifyOfPropertyChange(nameof(this.CanLogin));
        }

        public void AddConnectionItem()
        {
            var dialog = new ConnectionItemEditViewModel();
            if (dialog.ShowDialog() == true)
            {
                this.connectionItems.Add(dialog.ConnectionInfo);
                this.connectionItems.Write();
            }
        }

        public void RemoveConnectionItem(ConnectionItemViewModel connectionItem)
        {
            if (this.ConnectionItem == connectionItem)
            {
                this.ConnectionItem = null;
            }
            this.connectionItems.Remove(connectionItem);
        }

        public void RemoveConnectionItem()
        {
            if (AppMessageBox.ConfirmDelete() == false)
                return;
            this.RemoveConnectionItem(this.ConnectionItem);
        }

        public void EditConnectionItem()
        {
            this.EditConnectionItem(this.ConnectionItem);
        }

        public void EditConnectionItem(ConnectionItemViewModel connectionItem)
        {
            var dialog = new ConnectionItemEditViewModel(connectionItem.Clone());
            if (dialog.ShowDialog() == true)
            {
                connectionItem.Assign(dialog.ConnectionInfo);
                this.connectionItems.Write();
                if (this.ConnectionItem == connectionItem)
                {
                    FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor = connectionItem.ThemeColor;
                    FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource = themes[connectionItem.Theme];
                    this.SetPassword(this.ConnectionItem.Password, true);
                }
            }
        }

        public void SetPassword(string password, bool isEncrypted)
        {
            if (string.IsNullOrEmpty(password) == true)
            {
                this.securePassword = null;
            }
            else
            {
                this.securePassword = new SecureString();
                if (isEncrypted == true)
                {
                    foreach (var item in StringUtility.Decrypt(password, this.ConnectionItem.ID))
                    {
                        this.securePassword.AppendChar(item);
                    }
                }
                else
                {
                    foreach (var item in password)
                    {
                        this.securePassword.AppendChar(item);
                    }
                }
            }
            this.isEncrypted = isEncrypted;
            this.NotifyOfPropertyChange(nameof(this.CanLogin));
        }

        public async void SelectDataBase(string dataBaseName)
        {
            try
            {
                if (this.isOpened == false)
                    throw new InvalidOperationException(Resources.Exception_CannotSelectWithoutLoggingIn);

                this.BeginProgress();
                if (this.DataBaseName != string.Empty)
                    await this.UnloadAsync();
                await this.LoadAsync(dataBaseName);
                this.EndProgress();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.EndProgress();
            }
        }

        public async Task LoadAsync(string dataBaseName)
        {
            var waiter = new TaskWaiter();
            var isProgressing = this.IsProgressing;
            this.OnLoading(EventArgs.Empty);

            if (isProgressing == false)
                this.BeginProgress();
            this.ProgressMessage = Resources.Message_LoadingDataBase;
            try
            {
                var autoLoad = this.authenticator.Authority == Authority.Admin && Keyboard.Modifiers == ModifierKeys.Shift;
                await this.EnterDataBaseAsync(dataBaseName);
                await waiter.WaitAsync(2000);
                this.dataBaseName = dataBaseName;
            }
            catch
            {
                if (isProgressing == false)
                    this.EndProgress();
                this.dataBase = null;
                this.dataBaseName = null;
                this.Refresh();
                throw;
            }

            this.IsLoaded = true;
            if (isProgressing == false)
                this.EndProgress();
            this.Refresh();
            this.OnLoaded(EventArgs.Empty);
        }

        public async void Unload()
        {
            await this.UnloadAsync();
        }

        public async Task UnloadAsync()
        {
            if (this.DataBaseName == string.Empty)
                throw new InvalidOperationException();

            await this.CloseDocumentsAsync(false);
            this.OnUnloading(EventArgs.Empty);
            this.BeginProgress();
            await this.LeaveDataBaseAsync();
            this.dataBaseName = null;
            this.IsLoaded = false;
            this.EndProgress();
            this.Refresh();
            this.OnUnloaded(EventArgs.Empty);
        }

        public bool IsLoaded
        {
            get { return this.isLoaded; }
            private set
            {
                this.isLoaded = value;
                this.NotifyOfPropertyChange(nameof(this.IsLoaded));
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public bool IsOpened
        {
            get { return this.isOpened; }
            private set
            {
                this.isOpened = value;
                this.NotifyOfPropertyChange(nameof(this.IsOpened));
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public bool HasError
        {
            get { return this.hasError; }
            set
            {
                this.hasError = value;
                this.NotifyOfPropertyChange(nameof(this.HasError));
            }
        }

        public string ErrorMessage
        {
            get { return this.ProgressMessage; }
            set
            {
                this.ProgressMessage = value;
                this.NotifyOfPropertyChange(nameof(this.ErrorMessage));
                this.HasError = (value ?? string.Empty) != string.Empty;
            }
        }

        public string DataBaseName
        {
            get { return this.dataBaseName ?? string.Empty; }
        }

        public string Address
        {
            get { return this.address ?? string.Empty; }
        }

        public Color ThemeColor
        {
            get { return this.themeColor; }
            set
            {
                this.themeColor = value;
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor = value;
                this.NotifyOfPropertyChange(nameof(ThemeColor));
            }
        }

        public string Theme
        {
            get { return this.theme; }
            set
            {
                var themeValue = Themes[value];
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource = themeValue;
                this.theme = Themes.First(item => item.Value == themeValue).Key;
                this.NotifyOfPropertyChange(nameof(Theme));
            }
        }

        public Authority Authority
        {
            get { return this.authority; }
        }

        public bool IsVisible
        {
            get { return this.IsOpened; }
        }

        public DataBaseServiceViewModel DataBaseService
        {
            get { return this.dataBaseService.Value; }
        }

        public ConnectionItemCollection ConnectionItems
        {
            get { return this.connectionItems; }
        }

        public ConnectionItemViewModel ConnectionItem
        {
            get { return this.connectionItem; }
            set
            {
                if (this.connectionItem == value)
                    return;

                this.ValidateSetConnectionItem(value);

                if (this.connectionItem != null)
                {
                    this.connectionItem.IsDefault = false;
                }
                this.connectionItem = value;
                if (this.connectionItem != null)
                {
                    this.connectionItem.IsDefault = true;
                    this.ThemeColor = this.connectionItem.ThemeColor;
                    this.Theme = this.connectionItem.Theme;
                    this.SetPassword(this.connectionItem.Password, true);
                }

                this.NotifyOfPropertyChange(nameof(this.CanLogin));
                this.NotifyOfPropertyChange(nameof(this.ConnectionItem));
                this.NotifyOfPropertyChange(nameof(this.CanRemoveConnectionItem));
                this.NotifyOfPropertyChange(nameof(this.CanEditConnectionItem));
            }
        }

        public IEnumerable<IDataBaseDescriptor> DataBases
        {
            get { return this.dataBaseSelections.Value.Items; }
        }

        public bool CanLogin
        {
            get
            {
                if (this.ConnectionItem == null)
                    return false;
                if (this.securePassword == null)
                    return false;
                if (this.IsProgressing == true)
                    return false;
                return true;
            }
        }

        public bool CanRemoveConnectionItem
        {
            get { return this.ConnectionItem != null; }
        }

        public bool CanEditConnectionItem
        {
            get { return this.ConnectionItem != null; }
        }

        public string FilterExpression
        {
            get { return this.filterExpression ?? string.Empty; }
            set
            {
                if (this.filterExpression == value)
                    return;

                if (this.FilterExpression == string.Empty)
                {
                    foreach (var item in this.connectionItems)
                    {
                        item.IsVisible = true;
                        item.Pattern = string.Empty;
                    }
                }

                this.filterExpression = value;

                if (this.FilterExpression == string.Empty)
                {
                    foreach (var item in this.connectionItems)
                    {
                        item.IsVisible = true;
                        item.Pattern = string.Empty;
                    }
                }
                else
                {
                    foreach (var item in this.connectionItems)
                    {
                        item.IsVisible = this.Filter(item);
                        item.Pattern = this.FilterExpression;
                        item.CaseSensitive = this.CaseSensitive;
                    }

                    if (this.connectionItem != null && this.connectionItem.IsVisible == false)
                        this.ConnectionItem = null;
                }

                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
            }
        }

        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set
            {
                this.caseSensitive = value;
                this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
            }
        }

        public bool GlobPattern
        {
            get { return this.globPattern; }
            set
            {
                this.globPattern = value;
                this.NotifyOfPropertyChange(nameof(this.GlobPattern));
            }
        }

        public ICremaConfiguration UserConfigs
        {
            get { return this.cremaHost.Configs; }
        }

        public string DisplayName => Resources.Title_Start;

        public event EventHandler Loading;

        public event EventHandler Loaded;

        public event EventHandler Unloading;

        public event EventHandler Unloaded;

        public event EventHandler Resetting;

        public event EventHandler Reset;

        public event EventHandler Opened;

        public event EventHandler Closed;

        protected virtual void OnLoading(EventArgs e)
        {
            this.Loading?.Invoke(this, e);
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        protected virtual void OnUnloading(EventArgs e)
        {
            this.Unloading?.Invoke(this, e);
        }

        protected virtual void OnUnloaded(EventArgs e)
        {
            this.Unloaded?.Invoke(this, e);
        }

        protected virtual void OnResetting(EventArgs e)
        {
            this.Resetting?.Invoke(this, e);
        }

        protected virtual void OnReset(EventArgs e)
        {
            this.Reset?.Invoke(this, e);
        }

        protected virtual void OnOpened(EventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        private async void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            this.isOpened = false;
            Trace.WriteLine("CremaHost_Closed");
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = false;
                this.EndProgress();
                if (e.Reason != CloseReason.None)
                    this.ErrorMessage = e.Message;
                else
                    this.ProgressMessage = e.Message;

                if (this.IsLoaded == true)
                {
                    this.dataBaseName = null;
                    this.IsLoaded = false;
                    this.OnUnloaded(EventArgs.Empty);
                }
                this.address = null;
                this.Refresh();
                this.OnClosed(EventArgs.Empty);
                this.cremaHost.SaveConfigs();
            });
        }

        private async void CremaHost_Opened(object sender, EventArgs e)
        {
            this.isOpened = true;
            this.authority = this.cremaHost.Authority;
            this.cremaHost.Closed += CremaHost_Closed;
            await this.Dispatcher.InvokeAsync((System.Action)(() =>
            {
                this.configs.Commit(this);
                this.Refresh();
            }));
        }

        private static string SecureStringToString(SecureString value, string userID)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return StringUtility.Encrypt(Marshal.PtrToStringUni(valuePtr), userID);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private async void DataBase_Unloaded(object sender, EventArgs e)
        {
            var dataBase = sender as IDataBase;
            dataBase.Unloaded -= DataBase_Unloaded;
            var dataBaseName = dataBase.Name;
            if (this.dataBaseName == dataBaseName)
            {
                this.isLoaded = false;
            }
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.dataBaseName == dataBaseName)
                {
                    this.dataBase = null;
                    this.dataBaseName = null;
                    this.IsLoaded = false;
                    this.Refresh();
                    this.OnUnloaded(EventArgs.Empty);
                }
            });
        }

        private void DataBase_Resetting(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.BeginProgress();
                this.OnResetting(EventArgs.Empty);
            });
        }

        private void DataBase_Reset(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() => 
            {
                this.OnReset(EventArgs.Empty);
                this.EndProgress();
            });
        }

        private async Task OpenAsync(string address, string userID, SecureString password)
        {
            await this.cremaHost.Dispatcher.InvokeAsync(() =>
            {
                this.token = this.cremaHost.Open(address, userID, password);
            });
            this.address = address;
            this.IsOpened = true;
            this.connectionItem.LastConnectedDateTime = DateTime.Now;
            this.ConnectionItems.Write();
            this.OnOpened(EventArgs.Empty);
        }

        private async Task CloseAsync()
        {
            this.cremaHost.Closed -= CremaHost_Closed;
            await this.cremaHost.Dispatcher.InvokeAsync(() =>
            {
                this.cremaHost.Close(this.token);
                this.token = Guid.Empty;
            });
            this.address = null;
            this.IsOpened = false;
            this.Refresh();
            this.OnClosed(EventArgs.Empty);
            this.cremaHost.SaveConfigs();
        }

        private Task EnterDataBaseAsync(string dataBaseName)
        {
            var autoLoad = this.authenticator.Authority == Authority.Admin && Keyboard.Modifiers == ModifierKeys.Shift;
            return this.cremaHost.Dispatcher.InvokeAsync(() =>
            {
                var dataBase = this.cremaHost.DataBases[dataBaseName];
                if (dataBase == null)
                    throw new ArgumentException(string.Format(Resources.Exception_NonExistentDataBase, dataBaseName), nameof(dataBaseName));
                if (dataBase.IsLoaded == false && autoLoad == true)
                {
                    dataBase.Load(this.authenticator);
                }
                dataBase.Enter(this.authenticator);
                dataBase.Unloaded += DataBase_Unloaded;
                dataBase.Resetting += DataBase_Resetting;
                dataBase.Reset += DataBase_Reset;
                this.dataBase = dataBase;
            });
        }

        private Task LeaveDataBaseAsync()
        {
            return this.cremaHost.Dispatcher.InvokeAsync(() =>
            {
                this.dataBase.Unloaded -= DataBase_Unloaded;
                this.dataBase.Resetting -= DataBase_Resetting;
                this.dataBase.Reset -= DataBase_Reset;
                this.dataBase.Leave(this.authenticator);
                this.dataBase = null;
            });
        }

        private async Task CloseDocumentsAsync(bool save)
        {
            var documentServices = this.cremaHost.GetService(typeof(IEnumerable<IDocumentService>)) as IEnumerable<IDocumentService>;
            var query = from documentService in documentServices
                        from document in documentService.Documents
                        select document;

            var documentList = query.ToList();
            foreach (var item in documentList.ToArray())
            {
                item.Disposed += (s, e) => documentList.Remove(item);
                await this.Dispatcher.InvokeAsync(() =>
                {
                    if (item.IsModified == true && save == false)
                        item.IsModified = false;
                    item.Dispose();
                });
                await Task.Delay(1);
            }

            while (documentList.Any())
            {
                await Task.Delay(1);
            }
        }

        private void ValidateSetConnectionItem(ConnectionItemViewModel value)
        {
            if (this.IsOpened == true)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.connectionItem.Address != value.Address || this.connectionItem.ID != value.ID || this.connectionItem.PasswordID != value.PasswordID)
                    throw new ArgumentException(Resources.Exception_InvalidConnectionItem, nameof(value));
            }
        }

        private bool Filter(ConnectionItemViewModel connectionItem)
        {
            if (this.Filter(connectionItem.Name) == true)
                return true;
            if (this.Filter(connectionItem.Address) == true)
                return true;
            if (this.Filter(connectionItem.DataBaseName) == true)
                return true;
            if (this.Filter(connectionItem.ID) == true)
                return true;
            return false;
        }

        private bool Filter(string text)
        {
            if (this.GlobPattern == true)
                return StringUtility.Glob(text, this.FilterExpression, this.CaseSensitive);
            else if (this.CaseSensitive == false)
                return text.IndexOf(filterExpression, StringComparison.OrdinalIgnoreCase) >= 0;
            return text.IndexOf(filterExpression) >= 0;
        }

        private IShell Shell => this.shell.Value;

        #region IPartImportsSatisfiedNotification

        async void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {

            });
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataBase))
                return this.dataBase;
            return this.cremaHost.GetService(serviceType);
        }

        #endregion

        #region ICremaAppHost

        void ICremaAppHost.Login(string address, string userID, string password, string dataBaseName)
        {
            var connectionItem = new ConnectionItemViewModel()
            {
                Name = "Temporary",
                Address = address,
                ID = userID,
                Password = StringUtility.Encrypt(password, userID),
                DataBaseName = dataBaseName,
                IsTemporary = true,
                Theme = this.Theme,
                ThemeColor = this.ThemeColor,
            };

            this.ConnectionItems.Add(connectionItem);
            this.ConnectionItem = connectionItem;
            this.Login();
        }

        IEnumerable<IConnectionItem> ICremaAppHost.ConnectionItems
        {
            get { return this.ConnectionItems; }
        }

        IConnectionItem ICremaAppHost.ConnectionItem
        {
            get { return this.ConnectionItem; }
            set
            {
                if (value is ConnectionItemViewModel connectionItem)
                {
                    this.ConnectionItem = connectionItem;
                }
            }
        }

        #endregion
    }
}
