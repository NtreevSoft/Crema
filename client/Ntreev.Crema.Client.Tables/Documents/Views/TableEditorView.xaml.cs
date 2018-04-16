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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Schema;
using System.IO;
using Ntreev.Crema.Data;
using System.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.Services.DomainService;
using Caliburn.Micro;
using Ntreev.Crema.Client.Tables.Documents.ViewModels;
using Ntreev.ModernUI.Framework.Controls;
using Xceed.Wpf.DataGrid;
using Ntreev.Crema.Services;
using System.ComponentModel.Composition;

namespace Ntreev.Crema.Client.Tables.Documents.Views
{
    /// <summary>
    /// Interaction logic for TableEditorView.xaml
    /// </summary>
    public partial class TableEditorView : UserControl, IDisposable
    {
        private readonly List<IDisposable> childViewList = new List<IDisposable>();
        //[Import]
        //private ICremaHost cremaHost = null;

        public TableEditorView()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
        }

        public void Dispose()
        {
            foreach (var item in this.childViewList)
            {
                item.Dispose();
            }
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            var contentControl = sender as ContentControl;
            if (contentControl.Content is IDisposable view && this.childViewList.Contains(view) == false)
            {
                this.childViewList.Add(view);
            }
        }
    }
}