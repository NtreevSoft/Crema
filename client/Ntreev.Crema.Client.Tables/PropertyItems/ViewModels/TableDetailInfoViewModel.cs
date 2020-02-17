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

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Crema.Services.Data;
using Ntreev.Library.Extensions;
using Ntreev.ModernUI.Framework;

namespace Ntreev.Crema.Client.Tables.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType(typeof(PropertyService))]
    public class TableDetailInfoViewModel : PropertyItemBase
    {
        private TableDetailInfo tableDetailInfo;
        private ITableDescriptor descriptor;

        public TableDetailInfoViewModel()
        {
            this.DisplayName = Resources.Title_TableDetailInfo;
        }

        public override bool CanSupport(object obj)
        {
            return obj is ITableDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.Detach();
            this.descriptor = obj as ITableDescriptor;
            this.Attach();
        }

        public override bool IsVisible => this.descriptor != null;
        public override object SelectedObject => this.descriptor;

        public TableDetailInfo TableDetailInfo
        {
            get => this.tableDetailInfo;
            set
            {
                this.tableDetailInfo = value;
                this.NotifyOfPropertyChange(nameof(this.TableDetailInfo));
            }
        }

        private void Attach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor.Target.TableDetailInfo == null)
                {
                    var tables = this.descriptor.Target.GetService<ITableCollection>();
                    var table = (Table) this.descriptor.Target;
                    table.Dispatcher.Invoke(() =>
                    {
                        ((TableCollection) tables).UpdateTableDetailInfo(new[] {table});
                    });
                }

                if (this.descriptor is INotifyPropertyChanged propertyChanged)
                {
                    propertyChanged.PropertyChanged += DescriptorOnPropertyChanged;
                }

                this.TableDetailInfo = this.descriptor.Target.TableDetailInfo;
            }

            this.NotifyOfPropertyChange(nameof(IsVisible));
            this.NotifyOfPropertyChange(nameof(SelectedObject));
        }

        private void Detach()
        {
            if (this.descriptor != null && this.descriptor is INotifyPropertyChanged propertyChanged)
            {
                propertyChanged.PropertyChanged -= DescriptorOnPropertyChanged;
            }
        }

        private void DescriptorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITableDescriptor.TableDetailInfo) || e.PropertyName == string.Empty)
            {
                this.TableDetailInfo = this.descriptor.Target.TableDetailInfo;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private static readonly BrushConverter BrushConverter = new BrushConverter();
        public static readonly Brush ServerTagBrush = (Brush)BrushConverter.ConvertFromString(TagInfoUtility.Server.Color);
        public static readonly Brush ClientTagBrush = (Brush)BrushConverter.ConvertFromString(TagInfoUtility.Client.Color);
        public static readonly Brush UnusedTagBrush = (Brush)BrushConverter.ConvertFromString(TagInfoUtility.Unused.Color);
    }
}
