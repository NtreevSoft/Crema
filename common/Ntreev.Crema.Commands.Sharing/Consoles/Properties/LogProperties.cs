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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Ntreev.Crema.ServiceModel;
using System.IO;

namespace Ntreev.Crema.Commands.Consoles.Properties
{
    [ResourceDescription("../Resources", IsShared = true)]
    static class LogProperties
    {
        [CommandProperty("quiet", 'q')]
        [DefaultValue(false)]
        public static bool IsQuiet
        {
            get; set;
        }

        [CommandProperty("limit", 'l')]
        [DefaultValue(-1)]
        public static int Limit
        {
            get; set;
        }

        public static void Print(TextWriter writer, LogInfo[] logs)
        {
            var count = 0;

            writer.WriteLine();
            writer.WriteLine(string.Empty.PadRight(Console.BufferWidth - 1, '='));
            
            foreach (var item in logs)
            {
                if (LogProperties.Limit >= 0 && LogProperties.Limit <= count)
                    break;

                using (TerminalColor.SetForeground(ConsoleColor.Cyan))
                {
                    writer.WriteLine($"Revision: {item.Revision}");
                }
                writer.WriteLine($"Author  : {item.UserID}");
                writer.WriteLine($"Date    : {item.DateTime}");
                if (IsQuiet == false)
                {
                    writer.WriteLine();
                    writer.WriteLine(item.Comment);
                }
                writer.WriteLine(string.Empty.PadRight(Console.BufferWidth - 1, '='));
                count++;
            }
            writer.WriteLine();
        }
    }
}
