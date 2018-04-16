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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Windows;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Console.Properties;

namespace Ntreev.Crema.Client.Console
{
    [Export(typeof(IContentService))]
    [InheritedExport(typeof(ConsoleViewModel))]
    [RequiredAuthority(Authority.Admin)]
    [Order(100)]
    class ConsoleViewModel : ScreenBase, IContentService
    {
        private readonly ICremaAppHost cremaAppHost;
        private bool isVisible;

        static ConsoleViewModel()
        {
            CommandSettings.IsConsoleMode = false;
        }

        [ImportingConstructor]
        public ConsoleViewModel(ICremaAppHost cremaAppHost, ConsoleView view)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.DisplayName = Resources.Title_Console;
        }

        public override string ToString()
        {
            return Resources.Title_Console;
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {
            this.IsVisible = true;
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }
    }
}