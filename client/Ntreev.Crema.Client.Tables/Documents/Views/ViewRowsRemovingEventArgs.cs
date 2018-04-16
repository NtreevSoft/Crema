using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ClientService.DomainService;
using System.Windows;

namespace Ntreev.Crema.Client.Contents.Views
{
    public class ViewRowsRemovingEventArgs : RoutedEventArgs
    {
        private readonly string[] keys;

        public ViewRowsRemovingEventArgs(RoutedEvent routedEvent, string[] keys)
            : base(routedEvent)
        {
            this.keys = keys;
        }

        public string[] Keys
        {
            get { return this.keys; }
        }
    }
}
