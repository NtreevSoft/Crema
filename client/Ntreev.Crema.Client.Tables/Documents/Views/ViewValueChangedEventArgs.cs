using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ClientService.DomainService;
using System.Windows;

namespace Ntreev.Crema.Client.Contents.Views
{
    /// <summary>
    /// <see cref="EventHandler{ViewValueChangedEventArgs}"/>이벤트에 대한 데이터를 제공합니다.
    /// </summary>
    public class ViewValueChangedEventArgs : RoutedEventArgs
    {
        private readonly RowValue rowValue;

        public ViewValueChangedEventArgs(RoutedEvent routedEvent, RowValue rowValue)
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
