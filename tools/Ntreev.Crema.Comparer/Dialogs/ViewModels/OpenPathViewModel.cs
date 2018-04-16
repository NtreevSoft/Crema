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

using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Comparer.Dialogs.ViewModels
{
    class OpenPathViewModel : ModalDialogBase
    {
        private string path1;
        private string path2;
        private string filterExpression;

        public OpenPathViewModel()
        {
            this.DisplayName = "폴더 선택";
        }

        public void SelectPath1()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                InitialDirectory = this.Path1,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Path1 = dialog.FileName;
            }
        }

        public void SelectPath2()
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                InitialDirectory = this.Path2,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Path2 = dialog.FileName;
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

        public void Open()
        {
            this.TryClose(true);
        }

        [ConfigurationProperty]
        public string Path1
        {
            get { return this.path1 ?? string.Empty; }
            set
            {
                this.path1 = value;
                this.NotifyOfPropertyChange(nameof(this.Path1));
                this.NotifyOfPropertyChange(nameof(this.CanOpen));
            }
        }

        public void Swap()
        {
            var path1 = this.Path1;
            this.Path1 = this.Path2;
            this.Path2 = path1;
        }

        [ConfigurationProperty]
        public string Path2
        {
            get { return this.path2 ?? string.Empty; }
            set
            {
                this.path2 = value;
                this.NotifyOfPropertyChange(nameof(this.Path2));
                this.NotifyOfPropertyChange(nameof(this.CanOpen));
            }
        }

        [ConfigurationProperty]
        public string FilterExpression
        {
            get { return this.filterExpression ?? string.Empty; }
            set
            {
                this.filterExpression = value;
                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
            }
        }

        public bool CanOpen
        {
            get
            {
                if (this.Path1 == this.Path2)
                    return false;
                return Directory.Exists(this.Path1) && Directory.Exists(this.Path2);
            }
        }
    }
}
