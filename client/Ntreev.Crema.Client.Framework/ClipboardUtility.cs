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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Framework
{
    public static class ClipboardUtility
    {
        public static string[][] GetData()
        {
            return GetData(false);
        }

        public static string[][] GetData(bool removeEmptyLine)
        {
            return GetLines(Clipboard.GetText(), removeEmptyLine);
        }

        private static string[][] GetLines(string text, bool removeEmptyLine)
        {
            var valuesArray = new List<string[]>();
            var lines = text.Split(new string[] { Environment.NewLine, }, StringSplitOptions.None);

            foreach (var item in lines)
            {
                var words = item.Split('\t').Select(w => CorrectionMultiline(w)).ToArray();

                if (removeEmptyLine == true && words.Where(i => i.Trim() != string.Empty).Any() == false)
                    continue;

                valuesArray.Add(words);
            }

            if (valuesArray.Count > 0)
            {
                var values = valuesArray.Last();
                if (values.Length == 1 && values[0] == string.Empty)
                {
                    valuesArray.RemoveAt(valuesArray.Count - 1);
                }
            }

            var maxColumns = valuesArray.Max(item => item.Length);

            for (int i = 0; i < valuesArray.Count; i++)
            {
                var values = valuesArray[i];

                if (values.Length == maxColumns)
                    continue;

                var rep = Enumerable.Repeat(string.Empty, maxColumns - values.Length);

                valuesArray[i] = values.Concat(rep).ToArray();
            }

            return valuesArray.ToArray();
        }

        private static string CorrectionMultiline(string text)
        {
            if (text.IndexOf('\n') > 0)
            {
                if (text.First() == '\"' && text.Last() == '\"')
                {
                    text = text.Substring(1);
                    text = text.Substring(0, text.Length - 1);
                }
                text = text.Replace("\"\"", "\"");
            }
            // 160 to 32
            return text.Replace(' ', ' ');
        }
    }
}
