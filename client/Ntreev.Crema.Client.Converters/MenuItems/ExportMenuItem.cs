﻿//Released under the MIT License.
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
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Input;
using Ntreev.Crema.Client.Converters.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework.ViewModels;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Converters.Properties;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Converters.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IToolMenuItem))]
    [Order(ExportMenuItem.Order)]
    public class ExportMenuItem : MenuItemBase
    {
        internal const int Order = 100;

        private readonly ICremaAppHost cremaAppHost;
        [Import]
        private Authenticator authenticator = null;

        [ImportingConstructor]
        public ExportMenuItem(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += this.InvokeCanExecuteChangedEvent;
            this.cremaAppHost.Unloaded += this.InvokeCanExecuteChangedEvent;
            this.DisplayName = Resources.MenuItem_Export;
        }

        protected override async void OnExecute(object parameter)
        {
            var dialog = await ExportViewModel.CreateInstanceAsync(this.authenticator, this.cremaAppHost);
            dialog?.ShowDialog();
        }

        protected override bool OnCanExecute(object parameter)
        {
            return this.cremaAppHost.IsLoaded;
        }
    }
}
