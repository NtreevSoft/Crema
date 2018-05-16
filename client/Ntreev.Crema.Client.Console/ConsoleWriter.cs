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

using Ntreev.Library.Commands;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Console
{
    class ConsoleWriter : StringWriter
    {
        private readonly TerminalControl control;

        public ConsoleWriter(TerminalControl control)
        {
            this.control = control;
            TerminalColor.ForegroundColorChanged += TerminalColor_ForegroundColorChanged;
            TerminalColor.BackgroundColorChanged += TerminalColor_BackgroundColorChanged;
        }

        private void TerminalColor_ForegroundColorChanged(object sender, EventArgs e)
        {
            var foregroundColor = TerminalColor.ForegroundColor;

            this.control.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor == null)
                    this.control.OutputForeground = null;
                else
                    this.control.OutputForeground = (Brush)this.control.FindResource(TerminalColors.FindForegroundKey(foregroundColor));
            });
        }

        private void TerminalColor_BackgroundColorChanged(object sender, EventArgs e)
        {
            var backgroundColor = TerminalColor.BackgroundColor;
            this.control.Dispatcher.InvokeAsync(() =>
            {
                if (backgroundColor == null)
                    this.control.OutputBackground = null;
                else
                    this.control.OutputBackground = (Brush)this.control.FindResource(TerminalColors.FindBackgroundKey(backgroundColor));
            });
        }

        public override void Write(char value)
        {
            base.Write(value);
            this.control.Dispatcher.Invoke(() => this.control.Append(value.ToString()));
        }

        public override void WriteLine()
        {
            base.WriteLine();
            this.control.Dispatcher.Invoke(() => this.control.AppendLine(string.Empty));
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            this.control.Dispatcher.Invoke(() => this.control.AppendLine(value));
        }

        public override void Write(string value)
        {
            base.Write(value);
            this.control.Dispatcher.Invoke(() => this.control.Append(value));
        }
    }
}
