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

using Ntreev.Crema.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Ntreev.Crema.ApplicationHost.Views
{
    /// <summary>
    /// Interaction logic for ConsoleView.xaml
    /// </summary>
    [Export]
    public partial class ConsoleView : UserControl, IPartImportsSatisfiedNotification
    {
        private CommandContext commandContext;

        [ImportingConstructor]
        public ConsoleView(CommandContext commandContext)
        {
            InitializeComponent();
            this.commandContext = commandContext;
            this.commandContext.Out = new ConsoleWriter(this.terminal);
            this.terminal.Prompt = "> ";
        }

        public async void Run(string commandLine)
        {
            try
            {
                await Task.Run(() => this.commandContext.Execute(this.commandContext.Name + " " + commandLine));
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (e.InnerException != null)
                    this.terminal.AppendLine(e.InnerException.Message);
                else
                    this.terminal.AppendLine(e.Message);
            }
            catch (Exception e)
            {
                this.terminal.AppendLine(e.Message);
            }
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.terminal.AppendLine("안녕하세요!");
                this.terminal.AppendLine("사용 가능한 명령들:");

                foreach (var item in this.commandContext.Commands)
                {
                    if (this.commandContext.IsCommandEnabled(item) == false)
                        continue;
                    this.terminal.AppendLine(" - " + item.Name);
                }

                this.terminal.AppendLine("도움말을 사용하시려면 'help'을(를) 입력하세요");
                this.terminal.AppendLine("버전을 확인하시려면 '--version'을(를) 입력하세요");
            }, DispatcherPriority.ApplicationIdle);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.terminal.Focus();
        }

        private void Terminal_Executed(object sender, RoutedEventArgs e)
        {
            this.Run(this.terminal.Text);
        }
    }
}
