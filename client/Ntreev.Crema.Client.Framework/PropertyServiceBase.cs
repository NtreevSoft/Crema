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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Framework
{
    public abstract class PropertyServiceBase : PropertyChangedBase, IPropertyService
    {
        private readonly ICremaAppHost cremaAppHost;
        private readonly IPropertyItem[] propertyItems;

        private IPropertyItem[] itemsSource;
        private object selectedObject;

        protected PropertyServiceBase(ICremaAppHost cremaAppHost, IEnumerable<IPropertyItem> propertyItems)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;

            this.propertyItems = MenuItemUtility.GetMenuItems(this, propertyItems).ToArray();
        }

        public object SelectedObject
        {
            get { return this.selectedObject; }
            set
            {
                if (this.selectedObject == value)
                    return;
                this.selectedObject = value;
                this.RefreshItems();
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public void RefreshItems()
        {
            foreach (var item in this.propertyItems)
            {
                if (item.CanSupport(this.selectedObject) == true)
                {
                    item.SelectObject(this.selectedObject);
                }
                else
                {
                    item.SelectObject(null);
                }
            }
        }

        public IEnumerable<IPropertyItem> Properties
        {
            get { return this.itemsSource; }
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {
            var query = from item in this.propertyItems
                        let attr = Attribute.GetCustomAttribute(item.GetType(), typeof(RequiredAuthorityAttribute), true) as RequiredAuthorityAttribute
                        where attr == null || this.cremaAppHost.Authority >= attr.Authority
                        select item;
            this.itemsSource = query.ToArray();
            this.SelectedObject = null;
            this.NotifyOfPropertyChange(nameof(this.Properties));
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.SelectedObject = null;
        }
    }
}
