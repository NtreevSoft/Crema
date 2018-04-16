using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Crema.ClientService.DomainService;
using System.Windows;

namespace Ntreev.Crema.Client.Contents.Views
{
    public class ViewRowRemovingEventArgs : RoutedEventArgs
    {
        private readonly string key;

        public ViewRowRemovingEventArgs(RoutedEvent routedEvent, string key)
            : base(routedEvent)
        {
            this.key = key;
        }

        public string Key
        {
            get { return this.key; }
        }
    }
}
