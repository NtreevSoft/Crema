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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ntreev.Crema.Presentation.Controls
{
    [TemplatePart(Name = "PART_Browsers", Type = typeof(FlowExpander))]
    [TemplatePart(Name = "PART_Properties", Type = typeof(FlowExpander))]
    [TemplatePart(Name = "PART_BrowsersSplitter", Type = typeof(DockPanelSplitter))]
    [TemplatePart(Name = "PART_PropertiesSplitter", Type = typeof(DockPanelSplitter))]
    public class ContentServiceControl : ContentControl
    {
        public static readonly DependencyProperty IsBrowserExpandedProperty =
            DependencyProperty.Register(nameof(IsBrowserExpanded), typeof(bool), typeof(ContentServiceControl),
                new FrameworkPropertyMetadata(true, IsBrowserExpandedPropertyChangedCallback));

        public static readonly DependencyProperty IsPropertyExpandedProperty =
            DependencyProperty.Register(nameof(IsPropertyExpanded), typeof(bool), typeof(ContentServiceControl),
                new FrameworkPropertyMetadata(true, IsPropertyExpandedPropertyChangedCallback));

        public static readonly DependencyProperty BrowserDistanceProperty =
            DependencyProperty.Register(nameof(BrowserDistance), typeof(double), typeof(ContentServiceControl),
                new FrameworkPropertyMetadata(250.0, BrowserDistancePropertyChangedCallback));

        public static readonly DependencyProperty PropertyDistanceProperty =
            DependencyProperty.Register(nameof(PropertyDistance), typeof(double), typeof(ContentServiceControl),
                new FrameworkPropertyMetadata(250.0, PropertyDistancePropertyChangedCallback));

        public static readonly DependencyProperty BrowserContentProperty =
            DependencyProperty.Register(nameof(BrowserContent), typeof(object), typeof(ContentServiceControl));

        public static readonly DependencyProperty PropertyContentProperty =
            DependencyProperty.Register(nameof(PropertyContent), typeof(object), typeof(ContentServiceControl));

        private FlowExpander browsers;
        private FlowExpander properties;
        private DockPanelSplitter browsersSplitter;
        private DockPanelSplitter propertiesSplitter;

        public ContentServiceControl()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.browsers = this.Template.FindName("PART_Browsers", this) as FlowExpander;
            this.properties = this.Template.FindName("PART_Properties", this) as FlowExpander;
            this.browsersSplitter = this.Template.FindName("PART_BrowsersSplitter", this) as DockPanelSplitter;
            this.propertiesSplitter = this.Template.FindName("PART_PropertiesSplitter", this) as DockPanelSplitter;

            if (this.browsersSplitter != null)
                this.browsersSplitter.PreviewMouseDoubleClick += BrowsersSplitter_PreviewMouseDoubleClick;
            if (this.propertiesSplitter != null)
                this.propertiesSplitter.PreviewMouseDoubleClick += PropertiesSplitter_PreviewMouseDoubleClick;

            if (this.browsers != null)
            {
                var binding = new Binding(nameof(IsBrowserExpanded)) { Source = this, Mode = BindingMode.TwoWay, };
                BindingOperations.SetBinding(this.browsers, FlowExpander.IsExpandedProperty, binding);
                this.browsers.SizeChanged += Browsers_SizeChanged;
            }

            if (this.properties != null)
            {
                var binding = new Binding(nameof(IsPropertyExpanded)) { Source = this, Mode = BindingMode.TwoWay, };
                BindingOperations.SetBinding(this.properties, FlowExpander.IsExpandedProperty, binding);
                this.properties.SizeChanged += Properties_SizeChanged;
            }
        }

        private void Properties_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.properties.IsExpanded == true)
            {
                this.PropertyDistance = this.properties.Width;
            }
        }

        private void Browsers_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.browsers.IsExpanded == true)
            {
                this.BrowserDistance = this.browsers.Width;
            }
        }

        public bool IsBrowserExpanded
        {
            get { return (bool)this.GetValue(IsBrowserExpandedProperty); }
            set { this.SetValue(IsBrowserExpandedProperty, value); }
        }

        public bool IsPropertyExpanded
        {
            get { return (bool)this.GetValue(IsPropertyExpandedProperty); }
            set { this.SetValue(IsPropertyExpandedProperty, value); }
        }

        public double BrowserDistance
        {
            get { return (double)this.GetValue(BrowserDistanceProperty); }
            set { this.SetValue(BrowserDistanceProperty, value); }
        }

        public double PropertyDistance
        {
            get { return (double)this.GetValue(PropertyDistanceProperty); }
            set { this.SetValue(PropertyDistanceProperty, value); }
        }

        public object BrowserContent
        {
            get { return this.GetValue(BrowserContentProperty); }
            set { this.SetValue(BrowserContentProperty, value); }
        }

        public object PropertyContent
        {
            get { return this.GetValue(PropertyContentProperty); }
            set { this.SetValue(PropertyContentProperty, value); }
        }

        private static void IsBrowserExpandedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ContentServiceControl).UpdateBrowserDistance();
        }

        private static void IsPropertyExpandedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ContentServiceControl).UpdatePropertyDistance();
        }

        private static void BrowserDistancePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ContentServiceControl).UpdateBrowserDistance();
        }

        private static void PropertyDistancePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ContentServiceControl).UpdatePropertyDistance();
        }

        private void UpdateBrowserDistance()
        {
            if (this.browsers != null && this.browsers.IsExpanded == true)
            {
                this.browsers.Width = this.BrowserDistance;
            }
        }

        private void UpdatePropertyDistance()
        {
            if (this.properties != null && this.properties.IsExpanded == true)
            {
                this.properties.Width = this.PropertyDistance;
            }
        }

        private void BrowsersSplitter_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && this.browsers != null)
            {
                this.browsers.Width = (double)BrowserDistanceProperty.DefaultMetadata.DefaultValue;
                e.Handled = true;
            }
        }

        private void PropertiesSplitter_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && this.properties != null)
            {
                this.properties.Width = (double)PropertyDistanceProperty.DefaultMetadata.DefaultValue;
                e.Handled = true;
            }
        }
    }
}
