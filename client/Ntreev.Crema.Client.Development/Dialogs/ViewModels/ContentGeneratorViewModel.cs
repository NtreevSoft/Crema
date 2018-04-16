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

using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ntreev.Library.Random;
using System.Threading;
using System.Threading.Tasks;
using Ntreev.Crema.Services.Random;
using Ntreev.Library.Linq;

namespace Ntreev.Crema.Client.Development.Dialogs.ViewModels
{
    public class ContentGeneratorViewModel : ModalDialogBase
    {
        private readonly ITableContent content;
        private readonly CancellationTokenSource tokenSource;
        private readonly Authenticator authenticator;

        public ContentGeneratorViewModel(ITableContent content)
        {
            this.content = content;
            this.authenticator = this.content.Table.GetService(typeof(Authenticator)) as Authenticator;
            this.tokenSource = new CancellationTokenSource();
            this.Initialize();
        }

        public void Stop()
        {
            this.tokenSource.Cancel();
            this.NotifyOfPropertyChange(nameof(this.CanStop));
        }

        public bool CanStop
        {
            get
            {
                return this.tokenSource.IsCancellationRequested == false;
            }
        }

        private async void Initialize()
        {
            try
            {
                await this.content.Dispatcher.InvokeAsync(() => this.content.EnterEdit(this.authenticator));
                var contents = EnumerableUtility.Friends(content, content.Childs);
                var errorCount = 0;
                for (var i = 0; i < 10000; i++)
                {
                    if (this.tokenSource.IsCancellationRequested == true)
                        break;

                    try
                    {
                        await this.content.Dispatcher.InvokeAsync(() => contents.Random().EditRandom(this.authenticator));
                    }
                    catch
                    {
                        errorCount++;
                        if (errorCount > 5)
                            break;
                    }
                }
                await this.content.Dispatcher.InvokeAsync(() => this.content.LeaveEdit(this.authenticator));
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
            }

            this.TryClose();
        }
    }
}
