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
using System.IO;
using System.Linq;
using System.Text;
using Ntreev.Crema.Reader.IO;
using System.Threading;

namespace Ntreev.Crema.Reader.Internal
{
    static class StringResource
    {
        private static int refCount = 0;
        private static Dictionary<int, string> strings = new Dictionary<int, string>();

        public static void Read(BinaryReader reader)
        {
            int stringCount = reader.ReadInt32();

            for (int i = 0; i < stringCount; i++)
            {
                int id = reader.ReadInt32();
                int length = reader.ReadInt32();

                if (strings.ContainsKey(id) == false)
                {
                    string text = string.Empty;
                    if (length != 0)
                    {
                        var bytes = reader.ReadBytes(length);
                        text = Encoding.UTF8.GetString(bytes);
                    }

                    strings[id] = text;
                }
                else
                {
                    reader.BaseStream.Seek(length, SeekOrigin.Current);
                }
            }
        }

        public static StringComparer GetComparer(bool caseSensitive)
        {
            if (caseSensitive == true)
                return StringComparer.CurrentCulture;
            return StringComparer.CurrentCultureIgnoreCase;
        }

        public static string GetString(int id)
        {
            return StringResource.strings[id];
        }

        public static bool Equals(int id, string s)
        {
            return StringResource.Equals(id, s, true);
        }

        public static bool Equals(int id, string s, bool caseSensitive)
        {
            string s1 = StringResource.GetString(id);

            return StringResource.GetComparer(caseSensitive).Equals(s1, s);
        }

        public static int Ref
        {
            get { return StringResource.refCount; }
            set
            {
                StringResource.refCount = value;
                if (StringResource.refCount == 0)
                    StringResource.strings.Clear();
            }
        }
    }
}
