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
using Ntreev.Crema.Data;
using Ntreev.Crema.Designer.ViewModels;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    class TypeViewerViewModel : DocumentBase, IPartImportsSatisfiedNotification
    {
        private readonly CremaDataType dataType;

        //private ObservableCollection<TypeItemViewModel> typeItems;
        //private TypeItemViewModel selectedItem;
        private object selectedItem;
        private string selectedColumn;
        private bool isLoaded;

        private bool isFinderVisible;

        public TypeViewerViewModel(CremaDataType dataType)
        {
            this.dataType = dataType;
        }

        public CremaDataType Type
        {
            get { return this.dataType; }
        }

        public object Source => this.dataType;

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                this.selectedItem = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            }
        }

        public int SelectedItemIndex
        {
            set
            {
                if (value >= 0 && value < this.dataType.View.Count)
                {
                    var item = this.dataType.View[value];
                    this.SelectedItem = item;
                }
                else
                {
                    this.SelectedItem = null;
                }
            }
        }

        public string SelectedColumn
        {
            get { return this.selectedColumn; }
            set
            {
                this.selectedColumn = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedColumn));
            }
        }


        //public ObservableCollection<TypeItemViewModel> Types
        //{
        //    get { return this.typeItems; }
        //}

        //public string SelectedTypeName
        //{
        //    get
        //    {
        //        if (this.selectedItem == null)
        //            return string.Empty;
        //        return this.selectedItem.ToString();
        //    }
        //    set
        //    {
        //        if (this.typeItems == null)
        //        {
        //            this.selectedTypeName = value;
        //            return;
        //        }

        //        var query = from item in this.typeItems
        //                    where item.ToString() == value
        //                    select item;

        //        if (query.Any() == false)
        //            return;

        //        this.SelectedItem = query.First();
        //    }
        //}

        //public TypeItemViewModel SelectedItem
        //{
        //    get { return this.selectedItem; }
        //    set
        //    {
        //        if (this.selectedItem != null)
        //        {
        //            this.selectedItem.IsVisible = false;
        //        }
        //        this.selectedItem = value;
        //        if (this.selectedItem != null)
        //        {
        //            this.selectedItem.IsVisible = true;

        //            var browser = this.serviceProvider.GetService(typeof(TypeBrowserViewModel)) as TypeBrowserViewModel;
        //            var viewModel = this.selectedItem.Type.ExtendedProperties[browser] as TypeTreeViewItemViewModel;
        //            browser.SelectedItem = viewModel;
        //        }
        //        this.NotifyOfPropertyChange(nameof(this.SelectedItem));
        //        this.OnSelectionChanged(EventArgs.Empty);
        //    }
        //}

        public bool IsLoaded
        {
            get { return this.isLoaded; }
            set
            {
                this.isLoaded = value;
                this.NotifyOfPropertyChange(nameof(this.IsLoaded));
            }
        }

        public bool IsFinderVisible
        {
            get { return this.isFinderVisible; }
            set
            {
                this.isFinderVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsFinderVisible));
            }
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        protected override Task CloseAsync()
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //foreach (var item in this.typeItems)
                //{
                //    item.Dispose();
                //}
                //this.typeItems = null;
            }).Task;
        }

        private void Initialize()
        {
            try
            {
                this.BeginProgress();
                this.DisplayName = this.dataType.Name;

                //var typeItemList = new List<TypeItemViewModel>
                //{
                //    new TypeItemViewModel(this.dataType)
                //};
                //foreach (var item in this.dataType.Childs)
                //{
                //    typeItemList.Add(new TypeItemViewModel(item));
                //}

                //var compositionService = this.serviceProvider.GetService(typeof(ICompositionService)) as ICompositionService;
                //foreach (var item in typeItemList)
                //{
                //    compositionService.SatisfyImportsOnce(item);
                //}

                //this.typeItems = new ObservableCollection<TypeItemViewModel>(typeItemList);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.EndProgress();
                this.TryClose();
                return;
            }

            //if (this.selectedTypeName != null)
            //    this.selectedItem = this.typeItems.Where(item => item.Name == this.selectedTypeName).First();
            //else
            //    this.selectedItem = this.typeItems.First();

            //this.selectedItem.IsVisible = true;
            //this.selectedTypeName = null;

            this.EndProgress();
            this.IsLoaded = true;
            //this.NotifyOfPropertyChange(nameof(this.Types));
            //this.NotifyOfPropertyChange(nameof(this.SelectedItem));
            this.NotifyOfPropertyChange(nameof(this.IsProgressing));
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.Initialize();
        }

        #endregion

        //#region ITypeDocument

        //IEnumerable<ITypeDocumentItem> ITypeDocument.TypeItems
        //{
        //    get { return this.typeItems; }
        //}

        //ITypeDocumentItem ITypeDocument.SelectedItem
        //{
        //    get { return this.selectedItem; }
        //}

        //#endregion
    }
}
