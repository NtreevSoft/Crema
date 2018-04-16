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

using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.Crema.Designer.Views
{
    /// <summary>
    /// Types.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TypesView : Selector
    {
        public TypesView()
        {
            InitializeComponent();
            this.ContentLoader = new ItemsViewContentLoader(this);
        }

        #region ListWidth
        public GridLength ListWidth
        {
            get { return (GridLength)GetValue(ListWidthProperty); }
            set { SetValue(ListWidthProperty, value); }
        }

        public static readonly DependencyProperty ListWidthProperty =
            DependencyProperty.Register("ListWidth", typeof(GridLength), typeof(TypesView),
            new PropertyMetadata(new GridLength(170)));
        #endregion

        #region ContentLoader
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }

        public static readonly DependencyProperty ContentLoaderProperty =
            DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(TypesView),
            new PropertyMetadata(new DefaultContentLoader()));
        #endregion

        public class ItemsViewContentLoader : DefaultContentLoader, IContentLoader
        {
            private readonly TypesView itemsView;
            private Dictionary<Uri, object> contents = new Dictionary<Uri, object>();

            public ItemsViewContentLoader(TypesView itemsView)
            {
                this.itemsView = itemsView;
            }

            protected override object LoadContent(Uri uri)
            {
                if (this.contents.ContainsKey(uri) == false)
                {
                    foreach (var item in this.itemsView.ItemsSource)
                    {
                        if (item.ToString() == uri.ToString())
                        {
                            var itemView = new TypeItemView()
                            {
                                DataContext = item,
                            };
                            Caliburn.Micro.ViewModelBinder.Bind(item, itemView, null);
                            this.contents.Add(uri, itemView);
                            break;
                        }
                    }
                }

                return this.contents[uri];
            }
        }
    }
}
