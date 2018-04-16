using Ntreev.Crema.ServiceModel.Data;
using Ntreev.Crema.ServiceModel.Data.Xml.Schema;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;
using System.Xml.Linq;
using Xceed.Wpf.DataGrid;

namespace Ntreev.Crema.Client.Framework.Controls
{
    /// <summary>
    /// DataTableControl.xaml에 대한 상호 작용 논리
    /// </summary>
    [TemplatePart(Name = "PART_DataGridControl", Type = typeof(ModernDataGridControl))]
    public partial class DataTableControl : UserControl
    {
        private ModernDataGridControl dataGridControl;
        public DataTableControl()
        {
            InitializeComponent();
            
        }

        #region ItemsSource
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DataTableControl));
        #endregion

        #region ReadOnly
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(DataTableControl));
        #endregion

        public static readonly RoutedEvent ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged", 
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DataTableControl));

        public event RoutedEventHandler ItemsSourceChanged
        {
            add { AddHandler(ItemsSourceChangedEvent, value); }
            remove { RemoveHandler(ItemsSourceChangedEvent, value); }
        }

        private void RaiseItemsSourceChangedEvent()
        {
            this.OnItemsSourceChanged(new RoutedEventArgs(DataTableControl.ItemsSourceChangedEvent));
        }

        protected virtual void OnItemsSourceChanged(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void TagSelector_ValueChanged(object sender, RoutedEventArgs e)
        {

        }

        private void dataGridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            var bindingList = this.dataGridControl.ItemsSource as DataGridCollectionView;

            if (bindingList == null)
                return;

            var dataView = bindingList.SourceCollection as DataView;
            //dataView.ListChanged += dataView_ListChanged;

            (bindingList.SourceCollection as IBindingList).ListChanged += dataView_ListChanged;
            //var c1 = this.Resources["wer"];
            //var c2 = this.Resources["wer"];
            
            foreach (var item in this.dataGridControl.DetailConfigurations)
            {
                //var relation = dataView.Table.DataSet.Relations[item.RelationName];
                item.Title = XName.Get(item.RelationName).LocalName;
                item.TitleTemplate = this.FindResource("Title_Template") as DataTemplate;


                item.Columns.Add(this.Resources["tagColumn"] as ColumnBase);
                item.Columns.Add(this.Resources["enableColumn"] as ColumnBase);
                item.Columns.Add(this.Resources["modifierColumn"] as ColumnBase);
                item.Columns.Add(this.Resources["modifiedDateTimeColumn"] as ColumnBase);

                //item.Columns.CollectionChanged += (s1, e1) =>
                //{
                //    int index = 0;
                //    foreach (var item1 in relation.ChildTable.Columns.OrderedColumns())
                //    {
                //        var column = item.Columns[item1.ColumnName];
                //        if (column != null)
                //        {
                //            column.VisiblePosition = index++;
                //        }
                //    }
                //    item.SetValue(Xceed.Wpf.DataGrid.Views.TableView.FixedColumnCountProperty, relation.ChildTable.PrimaryKey.Length + 2);
                //};
            }

            //this.view.FixedColumnCount = dataView.Table.PrimaryKey.Length + 2;
            //this.dataGridControl.AdjustColumnsWidth();

            //this.modifierColumn.VisiblePosition = 10000;
            //this.modifiedDateTimeColumn.VisiblePosition = 10001;
            //(this.dataGridControl.View as Views.TableView).FixedColumnCount = this.ItemsSource.PrimaryKey.Count() + 2;

            //this.dataGridControl.ExpandDetails(this.dataGridControl.Items[0]);
            //this.dataGridControl.CollapseDetails(this.dataGridControl.Items[0]);

            foreach (var item in this.dataGridControl.DetailConfigurations)
            {
                item.Columns[CremaSchema.Modifier].VisiblePosition = item.Columns.Count - 1;
                item.Columns[CremaSchema.ModifiedDateTime].VisiblePosition = item.Columns.Count - 1;
                item.SetValue(Xceed.Wpf.DataGrid.Views.TableView.FixedColumnCountProperty, 3);
            }
            
        }

        private void dataView_ListChanged(object sender, ListChangedEventArgs e)
        {
            //if (e.ListChangedType == ListChangedType.Reset)
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        //this.dataGridControl.AdjustColumnsWidth();
            //        this.RaiseItemsSourceChangedEvent();
            //    }, DispatcherPriority.Render);
            //}
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = this.dataGridControl.ItemsSource as DataGridCollectionView;
            if (source == null)
                return;

            var dataView = source.SourceCollection as ITypedList;

            if (dataView != null && e.Action == NotifyCollectionChangedAction.Reset)
            {
                int index = 0;
                foreach (PropertyDescriptor item in dataView.GetItemProperties(null))
                {
                    var column = this.dataGridControl.Columns[item.Name];
                    if (column != null)
                    {
                        column.VisiblePosition = index++;
                    }

                    if (item.PropertyType.IsEnum == true)
                    {
                        var selector = this.dataGridControl.FindResource("EnumSelector");
                        column.CellEditor = selector as CellEditor;
                        //column.SetValue(
                        //int qwer = 0;
                    }
                }
                //this.view.FixedColumnCount = dataView.Table.PrimaryKey.Length + 2;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Released)
                return;

            var element = this.dataGridControl.InputHitTest(e.GetPosition(this.dataGridControl)) as DependencyObject;

            while (element != null)
            {
                if (element is ModernDataCell)
                    break;
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                var cell = element as ModernDataCell;
                //cell.IsSelecting = true;
                //Console.WriteLine(element);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dataGridControl = this.Template.FindName("PART_DataGridControl", this) as ModernDataGridControl;

            this.dataGridControl.Columns.Add(this.Resources["tagColumn"] as ColumnBase);
            this.dataGridControl.Columns.Add(this.Resources["enableColumn"] as ColumnBase);
            this.dataGridControl.Columns.Add(this.Resources["modifierColumn"] as ColumnBase);
            this.dataGridControl.Columns.Add(this.Resources["modifiedDateTimeColumn"] as ColumnBase);

            this.dataGridControl.Columns.CollectionChanged += Columns_CollectionChanged;
            (this.dataGridControl.VisibleColumns as INotifyCollectionChanged).CollectionChanged += VisibleColumns_CollectionChanged;
            this.dataGridControl.ItemsSourceChangeCompleted += dataGridControl_ItemsSourceChangeCompleted;
        }

        private void VisibleColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var source = this.dataGridControl.ItemsSource as DataGridCollectionView;
            if (source == null)
                return;

            var dataView = source.SourceCollection as DataView;

            //if (dataView != null && e.Action == NotifyCollectionChangedAction.Reset)
            //{
            //    this.view.FixedColumnCount = dataView.Table.PrimaryKey.Length + 2;
            //    this.dataGridControl.AdjustColumnsWidth();
            //}
        }

        private void DataGridCollectionViewSource_NewItemCreated(object sender, DataGridItemEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //var viewSource = this.Resources["viewSource"] as DataGridCollectionViewSource;

            //var typedList = viewSource.Source as ITypedList;

            //foreach (PropertyDescriptor item in typedList.GetItemProperties(null))
            //{
            //    if (item.IsBrowsable == false)
            //        continue;

            //    viewSource.ItemProperties.Add(new DataGridItemProperty(item));

            //    if (typeof(IBindingList).IsAssignableFrom(item.PropertyType) == false)
            //    {
            //        Column column = new Column()
            //        {
            //            FieldName = item.Name,
            //            Title = item.DisplayName,
            //        };
            //        if (this.dataGridControl.Columns[item.Name] == null)
            //            this.dataGridControl.Columns.Add(column);
            //    }
            //}
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            
        }
    }
}
