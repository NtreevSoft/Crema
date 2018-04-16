using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ClientService.DomainService;
using System.Windows;

namespace Ntreev.Crema.Client.Contents.Views
{
    public class ViewNewRowInsertingEventArgs : RoutedEventArgs
    {
        readonly RowValue rowValue;

        public ViewNewRowInsertingEventArgs(RoutedEvent routedEvent, RowValue rowValue)
            : base(routedEvent)
        {
            this.rowValue = rowValue;
        }

        public RowValue RowValue
        {
            get { return this.rowValue; }
        }
    }
}
