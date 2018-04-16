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

using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework
{
    public abstract class ListBoxBase<T> : ListBoxViewModel<T> where T : ListBoxItemViewModel
    {
        private const int defaultDelay = 100;
        private readonly List<IPropertyService> propertyServiceList = new List<IPropertyService>();
        private Task selectPropertyTask;
        private int delay = defaultDelay;

        public void AttachPropertyService(IPropertyService propertyService)
        {
            this.propertyServiceList.Add(propertyService);
        }

        public void DetachPropertyService(IPropertyService propertyService)
        {
            this.propertyServiceList.Remove(propertyService);
        }

        protected async override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            if (this.propertyServiceList.Any() == true)
            {
                await this.SelectPropertyAsync();
            }
        }

        protected virtual bool Predicate(IPropertyService propertyService)
        {
            return true;
        }

        private Task SelectPropertyAsync()
        {
            if (this.selectPropertyTask == null)
            {
                this.selectPropertyTask = this.Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(this.delay);
                    foreach (var item in this.propertyServiceList)
                    {
                        if (this.Predicate(item) == true)
                        {
                            item.SelectedObject = this.SelectedItem;
                        }
                    }
                    this.selectPropertyTask = null;
                }).Task;
            }
            return this.selectPropertyTask;
        }
    }
}
