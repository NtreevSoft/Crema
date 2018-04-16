using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.Client.Framework;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Ntreev.Crema.Model;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Client.Browsers.ViewModels
{
    //[Export(typeof(IBrowser))]
    [Order(0)]
    [RequiredAuthority(Authority.Guest)]
    public class CollectionBrowserViewModel : PropertyChangedBase, IBrowser
    {
        private readonly ClientModel _clienModel;
        private readonly IEventAggregator _events;
        private bool _visible;

        public ObservableCollection<CollectionTreeViewItemViewModel> Collections { get; private set; }

        [ImportingConstructor]
        public CollectionBrowserViewModel(ClientModel clientModel, IEventAggregator events)
        {
            this._clienModel = (ClientModel)clientModel;
            this._events = events;
            this._visible = true;

            this.Collections = new ObservableCollection<CollectionTreeViewItemViewModel>();

            this.Collections.Add(new CollectionTreeViewItemViewModel(
                new Collection() { Name = "2011년 이후 수정 테이블" }));
            this.Collections.Add(new CollectionTreeViewItemViewModel(
                new Collection() { Name = "색상" }));
            this.Collections.Add(new CollectionTreeViewItemViewModel(
                new Collection() { Name = "E_ItemType 사용 테이블" }));
        }
        
        public string Name
        {
            get { return "컬렉션"; }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                NotifyOfPropertyChange(() => Visible);
            }
        }
    }
}
