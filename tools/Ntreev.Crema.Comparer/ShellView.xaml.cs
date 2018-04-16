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
using Ntreev.Crema.Comparer.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ntreev.Crema.Comparer
{
    /// <summary>
    /// ShellView.xaml에 대한 상호 작용 논리
    /// </summary>
    [Export]
    public partial class ShellView : ModernWindow
    {
        public static readonly DependencyProperty IsProgressingProperty =
            DependencyProperty.Register(nameof(IsProgressing), typeof(bool), typeof(ShellView));

        [Import]
        private ShellViewModel shell = null;

        public ShellView()
        {
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Execute, Open_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, Close_Execute, Close_CanExecute));
        }

        [ImportingConstructor]
        public ShellView(ContentLoader contentLoader)
            : this()
        {
            this.ContentLoader = contentLoader;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public bool IsProgressing
        {
            get { return (bool)this.GetValue(IsProgressingProperty); }
            set { this.SetValue(IsProgressingProperty, value); }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            var effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                var items = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (items.Length == 2)
                {
                    if (Directory.Exists(items[0]) == true && Directory.Exists(items[1]) == true)
                    {
                        effects = DragDropEffects.Move;
                    }
                }
            }
            e.Effects = effects;
            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                var items = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (items.Length == 2)
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new OpenPathViewModel()
                        {
                            Path1 = items[0],
                            Path2 = items[1],
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            this.shell.Open(dialog.Path1, dialog.Path2, dialog.FilterExpression);
                        }
                    });
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }
        
        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.shell.Open();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.shell.CanOpen;
        }

        private void Close_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.shell.Close();
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.shell.CanClose;
        }
    }
}
