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

using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.Documents.ViewModels;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.DataGrid.Controls;
using Xceed.Wpf.AvalonDock.Controls;

namespace Ntreev.Crema.Client.Tables.MenuItems.TableMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(TableMenuItem))]
    public class NumberCellFormattingMenuItem : MenuItemBase
    {
        private readonly IShell shell;
        private readonly IAppConfiguration configService;

        [Import]
        private TableServiceViewModel tableService = null;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public NumberCellFormattingMenuItem(IShell shell, IAppConfiguration configService)
        {
            this.shell = shell;
            this.configService = configService;

            this.shell.ServiceChanged += this.InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_NumberCellFormatting;

            this.configService.Update(this);
            this.IsChecked = this.IsNumberFormatting;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.shell.SelectedService == this.tableService;
        }

        protected override void OnExecute(object parameter)
        {
            base.OnExecute(parameter);

            this.IsNumberFormatting = !this.IsChecked;
            this.IsChecked = this.IsNumberFormatting;

            this.configService[this.GetType(), nameof(IsNumberFormatting)] = this.IsNumberFormatting;
            this.configService.Commit(this);

            foreach (var item in this.tableService.DocumentService.Documents.ToArray())
            {
                if (item is TableDocumentBase document)
                {
                    if (document?.GetView() is FrameworkElement element)
                    {
                        var dataGridControls = element.FindVisualChildren<ModernDataGridControl>();
                        foreach (var control in dataGridControls)
                        {
                            control.IsNumberFormatting = this.IsNumberFormatting;
                        }
                    }
                }
            }
        }

        [ConfigurationProperty]
        public bool IsNumberFormatting { get; set; }
    }
}
