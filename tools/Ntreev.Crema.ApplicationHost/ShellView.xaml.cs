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

using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using Ntreev.Crema.Tools.Framework;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ntreev.Crema.ApplicationHost
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    [Export]
    public partial class ShellView : ModernWindow, IPartImportsSatisfiedNotification
    {
        [Import]
        private IContentService contentService = null;
        private ModernFrame contentFrame;

        static ShellView()
        {
            //ContentSourceProperty.OverrideMetadata(typeof(ShellView), new FrameworkPropertyMetadata(null, ContentSourcePropertyChangedCallback));
        }

        [ImportingConstructor]
        public ShellView(ContentLoader contentLoader)
        {
            this.InitializeComponent();
            //this.contentLoader = contentLoader;
            this.ContentLoader = contentLoader;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentFrame = this.Template.FindName("ContentFrame", this) as ModernFrame;

            if (this.contentFrame != null)
            {
                this.contentFrame.Navigated += ContentFrame_Navigated;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        private void ContentFrame_Navigated(object sender, FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            this.UpdateSource();
        }

        private void UpdateSource()
        {
            if (this.contentService == null)
                return;

            var view = this.contentFrame.Content as FrameworkElement;
            if (view == null)
                return;

            if (this.contentService is INotifyPropertyChanged == true)
            {
                (this.contentService as INotifyPropertyChanged).PropertyChanged -= ContentService_PropertyChanged;
            }
            this.contentService.SelectedContent = view.DataContext;
            if (this.contentService is INotifyPropertyChanged == true)
            {
                (this.contentService as INotifyPropertyChanged).PropertyChanged += ContentService_PropertyChanged;
            }
        }

        private void Contents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            this.AddContent(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            this.RemoveContent(item);
                        }
                    }
                    break;
            }
        }

        private void AddContent(object content)
        {
            var groupName = "etc";
            var displayName = string.Empty;
            var comment = string.Empty;

            if (content is IContent)
            {
                groupName = (content as IContent).GroupName;
                displayName = (content as IContent).DisplayName;
                comment = (content as IContent).Comment;
            }

            var group = this.MenuLinkGroups.FirstOrDefault(i => i.DisplayName == groupName);
            if (group == null)
            {
                group = new LinkGroup() { DisplayName = groupName, };
                this.MenuLinkGroups.Add(group);
            }

            var uri = new Uri("vm:///" + content.GetHashCode());
            var link = new ContentLink()
            {
                DisplayName = displayName,
                Source = uri,
                Content = content,
                Comment = comment,
            };

            group.Links.Add(link);

            if (content is INotifyPropertyChanged == true)
            {
                (content as INotifyPropertyChanged).PropertyChanged += Content_PropertyChanged;
            }
        }

        private void RemoveContent(object content)
        {
            foreach (var group in this.MenuLinkGroups)
            {
                foreach(var link in group.Links)
                {
                    if (link is ContentLink == true)
                    {
                        var contentLink = link as ContentLink;

                        if (contentLink.Content == content)
                        {
                            group.Links.Remove(link);

                            if (content is INotifyPropertyChanged == true)
                            {
                                (content as INotifyPropertyChanged).PropertyChanged -= Content_PropertyChanged;
                            }
                            return;
                        }
                    }
                }
            }
            //var links = this.MenuLinkGroups.SelectMany(i => i.Links).OfType<ContentLink>();
            //var link = links.FirstOrDefault(item => item.Content == content);

            //if (link != null)
            //{
            //    if (content is INotifyPropertyChanged == true)
            //    {
            //        (content as INotifyPropertyChanged).PropertyChanged += Content_PropertyChanged;
            //    }
            //}
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DisplayName")
            {
                var property = sender.GetType().GetProperty("DisplayName");
                var displayName = property.GetValue(sender);

                if (displayName is string)
                {
                    var links = this.MenuLinkGroups.SelectMany(i => i.Links).OfType<ContentLink>();
                    var link = links.FirstOrDefault(item => item.Content == sender);

                    if (link != null)
                    {
                        link.DisplayName = displayName as string;
                    }
                }
            }
            else if (e.PropertyName == "Comment")
            {
                var property = sender.GetType().GetProperty("Comment");
                var comment = property.GetValue(sender);

                if (comment is string)
                {
                    var links = this.MenuLinkGroups.SelectMany(i => i.Links).OfType<ContentLink>();
                    var link = links.FirstOrDefault(item => item.Content == sender);

                    if (link != null)
                    {
                        link.Comment = comment as string;
                    }
                }
            }
        }

        private void ContentService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedContent")
            {
                if (this.contentService.SelectedContent != null)
                {
                    var content = this.contentService.SelectedContent;
                    this.ContentSource = new Uri("vm:///" + content.GetHashCode());
                }
                else
                {
                    this.ContentSource = null;
                }
            }
        }

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.contentService.Contents)
            {
                this.AddContent(item);
            }

            var content = this.contentService.SelectedContent;
            this.ContentSource = new Uri("vm:///" + content.GetHashCode().ToString());

            this.contentService.Contents.CollectionChanged += Contents_CollectionChanged;

            if (this.contentService is INotifyPropertyChanged == true)
            {
                (this.contentService as INotifyPropertyChanged).PropertyChanged += ContentService_PropertyChanged;
            }
        }

        #endregion
    }
}
