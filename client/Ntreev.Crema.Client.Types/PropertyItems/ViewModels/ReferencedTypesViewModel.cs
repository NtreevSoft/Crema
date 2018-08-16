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
using System.Linq;
using System.ComponentModel.Composition;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using Ntreev.Library;
using System.Collections.Generic;
using System.Collections;
using Ntreev.ModernUI.Framework;
using System.ComponentModel;
using Ntreev.Library.Linq;
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.ModernUI.Framework.ViewModels;
using TypeDescriptor = Ntreev.Crema.Client.Framework.TypeDescriptor;
using Ntreev.Crema.Client.Types.BrowserItems.ViewModels;

namespace Ntreev.Crema.Client.Types.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [Dependency("Ntreev.Crema.Client.Tables.PropertyItems.ViewModels.TableInfoViewModel, Ntreev.Crema.Client.Tables, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [ParentType("Ntreev.Crema.Client.Tables.IPropertyService, Ntreev.Crema.Client.Tables, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null")]
    class ReferencedTypesViewModel : PropertyItemBase, ISelector
    {
        [Import]
        private Lazy<TypeBrowserViewModel> browser = null;
        [Import]
        private Authenticator authenticator = null;
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
            this.descriptor = obj as ITableDescriptor;

            if (this.descriptor != null)
            {
                var types = EnumerableUtility.Descendants<TreeViewItemViewModel, ITypeDescriptor>(this.Browser.Items, item => item.Items);
                var query = from column in this.descriptor.TableInfo.Columns
                            join type in types on column.DataType equals (type.TypeInfo.CategoryPath + type.TypeInfo.Name)
                            select type;
                var descriptors = query.Distinct().ToArray();
                var viewModelList = new List<TypeListBoxItemViewModel>();

                foreach (var item in descriptors)
                {
                    var type = item.Target;
                    if (type.ExtendedProperties.ContainsKey(this) == true)
                    {
                        var descriptor = type.ExtendedProperties[this] as TypeDescriptor;
                        viewModelList.Add(descriptor.Host as TypeListBoxItemViewModel);
                    }
                    else
                    {
                        var viewModel = new TypeListBoxItemViewModel(this.authenticator, item, this);
                        this.compositionService.SatisfyImportsOnce(viewModel);
                        viewModelList.Add(viewModel);
                    }
                }

                this.Types = viewModelList.ToArray();
            }
            else
            {
                this.Types = new TypeListBoxItemViewModel[] { };
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
                return this.types?.Any() == true;
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
                if (this.selectedType != null)
                    this.selectedType.IsSelected = false;
                this.selectedType = value;
                if (this.selectedType != null)
                    this.selectedType.IsSelected = true;
                this.NotifyOfPropertyChange(nameof(this.SelectedType));
            }
        }

        private TypeBrowserViewModel Browser => this.browser.Value;

        #region ISelector

        object ISelector.SelectedItem
        {
            get { return this.SelectedType; }
            set { this.SelectedType = value as TypeListBoxItemViewModel; }
        }

        #endregion
    }
}
