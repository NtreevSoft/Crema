using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ClientService.DomainService;
using System.Windows;

namespace Ntreev.Crema.Client.Contents.Views
{
    public class ViewNewRowsInsertingEventArgs : RoutedEventArgs
    {
        readonly RowValue[] rowValues;

        public ViewNewRowsInsertingEventArgs(RoutedEvent routedEvent, RowValue[] rowValues)
            : base(routedEvent)
        {
            this.rowValues = rowValues;
        }

        public RowValue[] RowValues
        {
            get { return this.rowValues; }
        }
    }
}
