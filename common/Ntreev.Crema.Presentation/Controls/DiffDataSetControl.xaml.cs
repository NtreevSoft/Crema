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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.Crema.Presentation.Controls
{
    /// <summary>
    /// DiffDataSetControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DiffDataSetControl : UserControl
    {
        public readonly static DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(DiffDataSet), typeof(DiffDataSetControl),
            new PropertyMetadata(null, SourcePropertyChangedCallback));

        private readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DiffDataSetControl));

        private readonly static DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DiffDataSetControl),
            new PropertyMetadata(null, SelectedItemPropertyChangedCallback));

        public DiffDataSetControl()
        {
            InitializeComponent();
        }

        public DiffDataSet Source
        {
            get { return (DiffDataSet)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        private static void SelectedItemPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldDependency = e.OldValue as DependencyObject;
            var newDependency = e.NewValue as DependencyObject;
            if (oldDependency != null)
            {
                oldDependency.SetValue(ItemViewModel.IsVisibleProperty, false);
            }

            if (newDependency != null)
            {
                newDependency.SetValue(ItemViewModel.IsVisibleProperty, true);
            }
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataSet = e.NewValue as DiffDataSet;

            if (dataSet != null)
            {
                var query = from item in dataSet.Tables
                                //where DiffDataSet.GetDiffState(item.DiffTable1) != DiffState.Unchanged
                            select new ItemViewModel(item, d);
                var itemsSource = query.ToArray();
                d.SetValue(ItemsSourceProperty, itemsSource);
                d.SetValue(SelectedItemProperty, itemsSource.FirstOrDefault());
            }
            else
            {
                d.SetValue(ItemsSourceProperty, null);
                d.SetValue(SelectedItemProperty, null);
            }
        }

        private IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        #region classes

        class ItemViewModel : DependencyObject
        {
            public readonly static DependencyProperty ReadOnlyProperty =
                DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ItemViewModel));

            public readonly static DependencyProperty IsVisibleProperty =
                DependencyProperty.Register("IsVisible", typeof(bool), typeof(ItemViewModel));

            private readonly DiffDataTable diffTable;
            private readonly DependencyObject parent;
            private readonly DiffState diffState;

            public ItemViewModel(DiffDataTable diffTable, DependencyObject parent)
            {
                this.diffTable = diffTable;
                this.parent = parent;
                this.diffState = DiffUtility.GetDiffState(this.diffTable.SourceItem1) == DiffState.Imaginary ? DiffState.Inserted : DiffUtility.GetDiffState(this.diffTable.SourceItem1);
                BindingOperations.SetBinding(this, ReadOnlyProperty, new Binding("ReadOnly") { Source = parent, });
            }

            public override string ToString()
            {
                return this.diffTable.ToString();
            }

            public DiffState DiffState
            {
                get { return this.diffState; }
            }

            public DiffDataTable Source
            {
                get { return this.diffTable; }
            }

            public bool ReadOnly
            {
                get { return (bool)this.GetValue(ReadOnlyProperty); }
                set { this.SetValue(ReadOnlyProperty, value); }
            }

            public bool IsVisible
            {
                get { return (bool)this.GetValue(IsVisibleProperty); }
                set { this.SetValue(IsVisibleProperty, value); }
            }
        }

        #endregion
    }
}
