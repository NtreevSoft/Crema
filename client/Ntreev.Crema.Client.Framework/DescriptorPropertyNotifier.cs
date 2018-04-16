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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Framework
{
    public class DescriptorPropertyNotifier
    {
        private readonly object target;
        private readonly Action<string> notifyAction;
        private readonly List<string> notifyList = new List<string>();
        private Dictionary<string, object> properties;
        private Task refreshTask;

        public DescriptorPropertyNotifier(object target, Action<string> notifyAction)
        {
            this.target = target;
            this.notifyAction = notifyAction;
        }

        public DescriptorPropertyNotifier(PropertyChangedBase target)
        {
            this.target = target;
            this.notifyAction = new Action<string>((item) => target.NotifyOfPropertyChange(item));
        }

        public Task RefreshAsync()
        {
            if (this.refreshTask == null)
            {
                this.refreshTask = Application.Current.Dispatcher.InvokeAsync(this.Refresh, DispatcherPriority.DataBind).Task;
            }
            return this.refreshTask;
        }

        public void Save()
        {
            var count = 0;
            foreach (var item in this.target.GetType().GetProperties())
            {
                if (item.CanRead == true && item.GetCustomAttribute<DescriptorPropertyAttribute>() != null)
                {
                    count++;
                }
            }
            this.properties = new Dictionary<string, object>(count);
            foreach (var item in this.target.GetType().GetProperties())
            {
                if (item.CanRead == true && item.GetCustomAttribute<DescriptorPropertyAttribute>() != null)
                {
                    this.properties.Add(item.Name, item.GetValue(this.target));
                }
            }
        }

        public void Refresh()
        {
            if (this.properties == null)
                throw new InvalidOperationException();

            foreach (var item in this.target.GetType().GetProperties())
            {
                if (this.properties.ContainsKey(item.Name) == false)
                    continue;

                if (item.CanRead == true && item.GetCustomAttribute<DescriptorPropertyAttribute>() != null)
                {
                    var oldValue = this.properties[item.Name];
                    var newValue = item.GetValue(this.target);
                    if (object.Equals(oldValue, newValue) == false)
                    {
                        this.properties[item.Name] = newValue;
                        this.notifyList.Add(item.Name);
                    }
                }
            }

            if (this.notifyList.Any() == true)
            {
#if DEBUG
                System.Diagnostics.Trace.WriteLine($"notified: {string.Join(", ", this.notifyList.ToArray())}");
#endif
                this.Notify();
            }
            this.refreshTask = null;
        }

        public async void UpdateProperty(object source, string propertyName)
        {
            var targetProp = this.target.GetType().GetProperty(propertyName);
            if (targetProp == null)
                return;
            var propAttr = targetProp.GetCustomAttribute<DescriptorPropertyAttribute>();
            var sourceProp = source.GetType().GetProperty(propertyName);
            var sourceValue = sourceProp.GetValue(source);
            if (propAttr.FieldName != string.Empty)
            {
                var fieldInfo = this.target.GetType().GetField(propAttr.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo.SetValue(this.target, sourceValue);
            }
            this.properties[propertyName] = sourceValue;
            this.notifyList.Add(propertyName);
            await this.RefreshAsync();
        }

        public bool IsReserved => this.refreshTask != null;

        private void Notify()
        {
            if (this.notifyList.Any() == true)
            {
                foreach (var item in this.notifyList)
                {
                    this.notifyAction(item);
                }
                this.notifyList.Clear();
            }
        }
    }
}
