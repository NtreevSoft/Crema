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

using Ntreev.Crema.Tools.Framework.Dialogs.ViewModels;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Tools.View.Dialogs.ViewModels
{
    class ConnectionViewModel : ModalDialogBase
    {
        private string address = "localhost";
        private string dataBase = "default";
        private string tags = "All";
        private bool isDevmode;
        private string filterExpression;

        public ConnectionViewModel()
        {
            this.DisplayName = "연결 설정";
        }

        public void SelectDataBase()
        {
            var dialog = new DataBaseListViewModel(this.Address);

            if (dialog.ShowDialog() == true)
            {
                this.DataBase = dialog.SelectedItem.Value.Name;
            }
        }

        public void EditFilterExpression()
        {
            var dialog = new EditFilterExpressionViewModel()
            {
                FilterExpression = this.FilterExpression,
            };
            if (dialog.ShowDialog() == true)
            {
                this.FilterExpression = dialog.FilterExpression;
            }
        }

        public void Connect()
        {
            this.TryClose(true);
        }

        [ConfigurationProperty("address")]
        public string Address
        {
            get { return this.address; }
            set
            {
                this.address = value;
                this.NotifyOfPropertyChange(() => this.Address);
                this.NotifyOfPropertyChange(() => this.CanConnect);
            }
        }

        [ConfigurationProperty("database")]
        public string DataBase
        {
            get { return this.dataBase; }
            set
            {
                this.dataBase = value;
                this.NotifyOfPropertyChange(() => this.DataBase);
                this.NotifyOfPropertyChange(() => this.CanConnect);
            }
        }

        [ConfigurationProperty("tags")]
        public string Tags
        {
            get { return this.tags; }
            set
            {
                this.tags = value;
                this.NotifyOfPropertyChange(() => this.Tags);
                this.NotifyOfPropertyChange(() => this.CanConnect);
            }
        }

        public bool CanConnect
        {
            get
            {
                if (this.tags == string.Empty)
                    return false;
                if (this.tags == TagInfo.Unused.ToString())
                    return false;
                if (this.dataBase == string.Empty)
                    return false;
                if (this.address == string.Empty)
                    return false;
                return true;
            }
        }

        [ConfigurationProperty("devmode")]
        public bool IsDevmode
        {
            get { return this.isDevmode; }
            set
            {
                this.isDevmode = value;
                this.NotifyOfPropertyChange(() => this.IsDevmode);
            }
        }

        [ConfigurationProperty("filter")]
        public string FilterExpression
        {
            get { return this.filterExpression ?? string.Empty; }
            set
            {
                this.filterExpression = value;
                this.NotifyOfPropertyChange(() => this.FilterExpression);
            }
        }
    }
}
