using Ntreev.Crema.ServiceModel;
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace Ntreev.Crema.Client.Framework.Controls
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TagSelector : UserControl
    {
        private static Dictionary<string, Color> tagToColor = new Dictionary<string, Color>();

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TagInfo), typeof(TagSelector),
            new UIPropertyMetadata(TagInfo.All, ValuePropertyChangedCallback, ValuePropertyCoerceValueCallback));

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(TagInfo), typeof(TagSelector),
            new UIPropertyMetadata(TagInfo.All, FilterPropertyChangedCallback));

        public static readonly DependencyProperty HideLabelProperty = DependencyProperty.Register("IsLabelVisible", typeof(bool), typeof(TagSelector),
            new UIPropertyMetadata(true));

        public static readonly RoutedEvent ValueChangedEvent = 
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagSelector));

        static TagSelector()
        {
            tagToColor.Add("Server", Color.FromArgb(255, 251, 190, 190));
            tagToColor.Add("Client", Color.FromArgb(255, 153, 183, 251));
            tagToColor.Add("Unused", Color.FromArgb(255, 220, 240, 220));
        }

        public TagSelector()
        {
            InitializeComponent();
        }

        public TagInfo Value
        {
            get { return (TagInfo)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public TagInfo Filter
        {
            get { return (TagInfo)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public bool IsLabelVisible
        {
            get { return (bool)GetValue(HideLabelProperty); }
            set { SetValue(HideLabelProperty, value); }
        }

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event EventHandler PopupClosed;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected virtual void OnPopupClosed(EventArgs e)
        {
            if (this.PopupClosed != null)
            {
                this.PopupClosed(this, e);
            }
        }

        private static object ValuePropertyCoerceValueCallback(DependencyObject d, object value)
        {
            TagInfo tag = (TagInfo)value;
            TagInfo filter = (TagInfo)d.GetValue(TagSelector.FilterProperty);
            return tag & filter;
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            element.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        private static void FilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ValueProperty, d.GetValue(ValueProperty));
        }

        private void selector_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();

            foreach (var item in TagInfo.Names)
            {
                var control = new MenuItem() { Header = item, };
                contextMenu.Items.Add(control);

                var value = TagInfo.Parse(item);

                if (tagToColor.ContainsKey(item) == true)
                    control.Background = new SolidColorBrush(tagToColor[item]);

                control.IsEnabled = (this.Filter & value) == value;
                control.IsChecked = this.Value == value;
                control.Click += control_Click;
            }

            contextMenu.Closed += contextMenu_Closed;
            contextMenu.PlacementTarget = this;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
            contextMenu.IsOpen = true;
         }

        private void contextMenu_Closed(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            contextMenu.Closed -= contextMenu_Closed;
            this.OnPopupClosed(EventArgs.Empty);
        }

        private void control_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item.IsChecked == true)
                return;

            this.Value = TagInfo.Parse(item.Header as string);
        }
    }
}
