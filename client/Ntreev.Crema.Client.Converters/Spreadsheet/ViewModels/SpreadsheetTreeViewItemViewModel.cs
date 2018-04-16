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

using Ntreev.Crema.Spreadsheet;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Client.Converters.Spreadsheet.ViewModels
{
    class SpreadsheetTreeViewItemViewModel : CheckableTreeViewItemViewModel
    {
        private readonly string path;
        private string errorString;

        public SpreadsheetTreeViewItemViewModel(string path)
        {
            this.path = path;
            this.Initialize();
        }

        public void Read(CremaDataSet dataSet)
        {
            using (var reader = new SpreadsheetReader(this.Path))
            {
                reader.Read(dataSet);
            }
        }

        public override string DisplayName
        {
            get { return this.path; }
        }

        public string Path
        {
            get { return this.path; }
        }

        public bool IsEnabled
        {
            get { return string.IsNullOrEmpty(this.errorString) == true; }
        }

        public string ErrorString
        {
            get { return this.errorString; }
            set
            {
                this.errorString = value;
                this.NotifyOfPropertyChange(nameof(this.IsEnabled));
                this.NotifyOfPropertyChange(nameof(this.IsChecked));
                this.NotifyOfPropertyChange(nameof(this.ErrorString));
            }
        }

        public string[] GetSelectedSheetNames()
        {
            return this.Items.Cast<SheetTreeViewItemViewModel>()
                             .Where(item => item.IsChecked == true)
                             .Select(item => item.SheetName)
                             .ToArray();
        }

        private void Initialize()
        {
            foreach (var item in SpreadsheetReader.ReadSheetNames(this.path))
            {
                var viewModel = new SheetTreeViewItemViewModel(item)
                {
                    Parent = this
                };
            }
        }
    }
}
