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

using Ntreev.Crema.Client.Framework;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Base.Services.ViewModels
{
    class ConnectionItemViewModel : ListBoxItemViewModel, IConnectionItem
    {
        private string name;
        private string address;
        private string dataBaseName;
        private string userID;
        private string password;
        private Color themeColor = FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor;
        private string theme;
        private bool isDefault;
        private bool isCurrentTheme;
        private bool isTemporary;
        private DateTime lastConnectedDateTime;

        [Import]
        private Lazy<CremaAppHostViewModel> cremaAppHost = null;

        public ConnectionItemViewModel()
        {

        }

        public ConnectionItemViewModel Clone()
        {
            return new ConnectionItemViewModel()
            {
                name = this.name,
                address = this.address,
                dataBaseName = this.dataBaseName,
                userID = this.userID,
                password = this.password,
                themeColor = this.themeColor,
                theme = this.theme,
                isDefault = this.isDefault,
                lastConnectedDateTime = this.lastConnectedDateTime,
            };
        }

        public void Assign(ConnectionItemViewModel connectionInfo)
        {
            this.name = connectionInfo.name;
            this.address = connectionInfo.address;
            this.dataBaseName = connectionInfo.dataBaseName;
            this.userID = connectionInfo.userID;
            this.password = connectionInfo.password;
            this.themeColor = connectionInfo.themeColor;
            this.theme = connectionInfo.theme;
            this.isDefault = connectionInfo.isDefault;
            this.lastConnectedDateTime = connectionInfo.lastConnectedDateTime;
            this.Refresh();
            this.RefreshIsCurrentThemeProperty();
        }

        public string Name
        {
            get { return this.name ?? string.Empty; }
            set
            {
                this.name = value;
                this.NotifyOfPropertyChange(nameof(this.Name));
            }
        }

        public string Address
        {
            get { return this.address ?? string.Empty; }
            set
            {
                this.address = value;
                this.NotifyOfPropertyChange(nameof(this.Address));
            }
        }

        public string DataBaseName
        {
            get { return this.dataBaseName ?? string.Empty; }
            set
            {
                this.dataBaseName = value;
                this.NotifyOfPropertyChange(nameof(this.DataBaseName));
            }
        }

        public string ID
        {
            get { return this.userID ?? string.Empty; }
            set
            {
                this.userID = value;
                this.NotifyOfPropertyChange(nameof(this.ID));
            }
        }

        public string Password
        {
            get
            {
                return this.password ?? string.Empty;
            }
            set
            {
                this.password = value;
                this.NotifyOfPropertyChange(nameof(this.Password));
            }
        }

        public Color ThemeColor
        {
            get { return this.themeColor; }
            set
            {
                this.themeColor = value;
                this.NotifyOfPropertyChange(nameof(this.ThemeColor));
            }
        }

        public string Theme
        {
            get { return this.theme ?? "Dark"; }
            set
            {
                this.theme = value;
                this.NotifyOfPropertyChange(nameof(this.Theme));
            }
        }

        public bool Equals(ConnectionItemViewModel dest)
        {
            if (this.Name != dest.Name)
                return false;
            if (this.Address != dest.Address)
                return false;
            if (this.DataBaseName != dest.DataBaseName)
                return false;
            if (this.ID != dest.ID)
                return false;
            if (this.themeColor != dest.themeColor)
                return false;
            if (this.theme != dest.theme)
                return false;
            return true;
        }

        public Guid PasswordID
        {
            get
            {
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                using (var md5 = MD5.Create())
                {
                    writer.Write(this.Password);
                    writer.Close();

                    var bytes = md5.ComputeHash(stream.GetBuffer());
                    var sBuilder = new StringBuilder();

                    for (var i = 0; i < bytes.Length; i++)
                    {
                        sBuilder.Append(bytes[i].ToString("x2"));
                    }

                    return Guid.Parse(sBuilder.ToString());
                }
            }
        }

        public bool IsDefault
        {
            get { return this.isDefault; }
            set { this.isDefault = value; }
        }

        public DateTime LastConnectedDateTime
        {
            get { return this.lastConnectedDateTime; }
            set { this.lastConnectedDateTime = value; }
        }

        public bool IsCurrentTheme
        {
            get { return this.isCurrentTheme; }
            private set
            {
                this.isCurrentTheme = value;
                this.NotifyOfPropertyChange(nameof(this.IsCurrentTheme));
            }
        }

        public bool IsTemporary
        {
            get { return this.isTemporary; }
            set
            {
                this.isTemporary = value;
                this.NotifyOfPropertyChange(nameof(this.IsTemporary));
            }
        }

        public static readonly ConnectionItemViewModel Empty = new ConnectionItemViewModel()
        {
            themeColor = FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor,
        };

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FirstFloor.ModernUI.Presentation.AppearanceManager.ThemeSource))
            {
                this.RefreshIsCurrentThemeProperty();
            }
        }

        private void RefreshIsCurrentThemeProperty()
        {
            foreach (var item in CremaAppHostViewModel.Themes)
            {
                if (item.Key == this.Theme && item.Key == this.CremaAppHost.Theme)
                {
                    this.IsCurrentTheme = true;
                    return;
                }
            }
            this.IsCurrentTheme = false;
        }

        private CremaAppHostViewModel CremaAppHost => this.cremaAppHost.Value;
    }
}
