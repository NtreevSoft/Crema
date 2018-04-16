using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Ntreev.Crema.Model;
using Ntreev.Crema.Library.WPF;

namespace Ntreev.Crema.Client.Browsers.ViewModels
{
    public class CollectionTreeViewItemViewModel : TreeViewItemViewModel, IComparable, IDisposable
    {
        private readonly Collection _collection;

        public CollectionTreeViewItemViewModel(Collection collection)
            : base(null)
        {
            _collection = collection;
        }

        public string Text
        {
            get { return _collection.Name; }
        }

        public Collection Collection
        {
            get { return _collection; }
        }

        public void Dispose()
        {
        }

        public int CompareTo(object obj)
        {
            CollectionTreeViewItemViewModel vm = obj as CollectionTreeViewItemViewModel;
            return this.Text.CompareTo(vm.Text);
        }
    }
}
