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

using Ntreev.Crema.Designer.Properties;
using Ntreev.Crema.Designer.Tables.ViewModels;
using Ntreev.Library;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [Dependency(typeof(TableInfoViewModel))]
    [ParentType(typeof(TablePropertyViewModel))]
    class ReferencedTypesViewModel : PropertyItemBase, ISelector
    {
        [Import]
        private Lazy<TypeBrowserViewModel> browser = null;
        private ITableDescriptor descriptor;
        private TypeListBoxItemViewModel[] types;
        private TypeListBoxItemViewModel selectedType;
        [Import]
        private ICompositionService compositionService = null;

        public ReferencedTypesViewModel()
        {
            this.DisplayName = Resources.Title_TypesBeingUsed;
        }

        public override bool CanSupport(object obj)
        {
            return obj is ITableDescriptor;
        }

        public override void SelectObject(object obj)
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged -= Table_PropertyChanged;
                }
            }

            this.descriptor = obj as ITableDescriptor;

            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged += Table_PropertyChanged;
                }

                var browser = this.browser.Value;
                var types = EnumerableUtility.Descendants<TreeViewItemViewModel, TypeTreeViewItemViewModel>(browser.Items, item => item.Items);
                var query = from item in this.descriptor.TableInfo.Columns
                            join type in types on item.DataType equals (type.TypeInfo.CategoryPath + type.TypeInfo.Name)
                            select type;
                var typeItems = query.Distinct()
                                     .Select(item => new TypeListBoxItemViewModel(item))
                                     .ToArray();

                foreach (var item in typeItems)
                {
                    compositionService.SatisfyImportsOnce(item);
                }
                this.Types = typeItems;
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        public override bool IsVisible
        {
            get
            {
                if (this.descriptor == null)
                    return false;
                return this.types != null && this.types.Any();
            }
        }

        public override object SelectedObject
        {
            get { return this.descriptor; }
        }

        public TypeListBoxItemViewModel[] Types
        {
            get { return this.types; }
            set
            {
                this.types = value;
                this.NotifyOfPropertyChange(nameof(this.Types));
            }
        }

        public TypeListBoxItemViewModel SelectedType
        {
            get { return this.selectedType; }
            set
            {
                this.selectedType = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedType));
            }
        }

        private void Table_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #region ISelector

        object ISelector.SelectedItem
        {
            get { return this.SelectedType; }
            set { this.SelectedType = value as TypeListBoxItemViewModel; }
        }

        #endregion
    }
}
