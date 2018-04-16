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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ntreev.Crema.Presentation.Controls
{
    public class CremaDataSetControl : Control
    {
        public readonly static DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(CremaDataSet), typeof(CremaDataSetControl),
            new PropertyMetadata(null, SourcePropertyChangedCallback));

        public readonly static DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(CremaDataSetControl));

        private readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CremaDataSetControl));

        private readonly static DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CremaDataSetControl),
            new PropertyMetadata(null, SelectedItemPropertyChangedCallback));

        public CremaDataSetControl()
        {

        }

        public CremaDataSet Source
        {
            get { return (CremaDataSet)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)this.GetValue(ReadOnlyProperty); }
            set { this.SetValue(ReadOnlyProperty, value); }
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CremaDataSet dataSet)
            {
                var query = from item in dataSet.Tables
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
                DependencyProperty.Register(nameof(ReadOnly), typeof(bool), typeof(ItemViewModel));

            public readonly static DependencyProperty IsVisibleProperty =
                DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(ItemViewModel));

            private readonly CremaDataTable source;
            private readonly DependencyObject parent;

            public ItemViewModel(CremaDataTable source, DependencyObject parent)
            {
                this.source = source;
                this.parent = parent;
                BindingOperations.SetBinding(this, ReadOnlyProperty, new Binding("ReadOnly") { Source = parent, });
            }

            public override string ToString()
            {
                return this.source.TableName;
            }

            public CremaDataTable Source
            {
                get { return this.source; }
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

            public TableInfo TableInfo
            {
                get { return this.source.TableInfo; }
            }
        }

        #endregion
    }
}
