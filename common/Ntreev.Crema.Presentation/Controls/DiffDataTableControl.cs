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

using Ntreev.Library.Linq;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.Crema.Presentation.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;
using System.Windows.Data;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Controls;
using System.Windows.Threading;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;

namespace Ntreev.Crema.Presentation.Controls
{
    public class DiffDataTableControl : Control
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(DiffDataTable), typeof(DiffDataTableControl),
                new PropertyMetadata(null, SourcePropertyChangedCallback));

        private static readonly DependencyPropertyKey TablesPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Tables), typeof(DiffDataTable[]), typeof(DiffDataTableControl),
                new PropertyMetadata(new DiffDataTable[] { }));
        public static readonly DependencyProperty TablesProperty = TablesPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectedTableProperty =
            DependencyProperty.Register(nameof(SelectedTable), typeof(DiffDataTable), typeof(DiffDataTableControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(DiffDataTableControl));

        public DiffDataTable Source
        {
            get { return (DiffDataTable)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public DiffDataTable[] Tables
        {
            get { return (DiffDataTable[])this.GetValue(TablesProperty); }
        }

        public DiffDataTable SelectedTable
        {
            get { return (DiffDataTable)this.GetValue(SelectedTableProperty); }
            set { this.SetValue(SelectedTableProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)this.GetValue(ReadOnlyProperty); }
            set { this.SetValue(ReadOnlyProperty, value); }
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DiffDataTable dataTable)
            {
                var items = EnumerableUtility.Friends(dataTable, dataTable.Childs);
                d.SetValue(TablesPropertyKey, items.ToArray());
                d.SetValue(SelectedTableProperty, items.First());
            }
            else
            {
                d.ClearValue(TablesPropertyKey);
                d.ClearValue(SelectedTableProperty);
            }
        }
    }
}
