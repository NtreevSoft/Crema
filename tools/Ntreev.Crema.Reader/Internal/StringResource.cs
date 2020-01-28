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
using Ntreev.Crema.Reader.Binary;

namespace Ntreev.Crema.Reader.Internal
{
    class StringValueDictionary
    {
        private readonly IDictionary<int, string> strings = new Dictionary<int, string>();

        public void Add(int key, string value)
        {
            this.strings.Add(key, value);
        }

        public bool ContainsKey(int id)
        {
            return this.strings.ContainsKey(id);
        }

        public string GetString(int id)
        {
            return this.strings[id];
        }

        public bool Equals(int id, string s)
        {
            return this.Equals(id, s, true);
        }

        public bool Equals(int id, string s, bool caseSensitive)
        {
            string s1 = this.GetString(id);

            return StringResource.GetComparer(caseSensitive).Equals(s1, s);
        }
    }

    static class StringResource
    {
        private static int refCount = 0;
        private static Dictionary<CremaBinaryTable, StringValueDictionary> tableStrings = new Dictionary<CremaBinaryTable, StringValueDictionary>();
        private static StringValueDictionary fileStrings = new StringValueDictionary();

        public static void ReadHeader(BinaryReader reader)
        {
            StringResource.Read(reader, null);
        }

        public static void Read(BinaryReader reader, CremaBinaryTable table)
        {
            int stringCount = reader.ReadInt32();
            var strings = GetTableStrings(table);

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

                    strings.Add(id, text);
                }
                else
                {
                    reader.BaseStream.Seek(length, SeekOrigin.Current);
                }
            }
        }

        public static StringValueDictionary GetHeaderStrings()
        {
            return StringResource.GetTableStrings(null);
        }

        public static StringValueDictionary GetTableStrings(CremaBinaryTable table)
        {
            if (table == null)
                return fileStrings;

            if (tableStrings.ContainsKey(table))
            {
                return tableStrings[table];
            }

            tableStrings.Add(table, new StringValueDictionary());
            return tableStrings[table];
        }

        public static StringComparer GetComparer(bool caseSensitive)
        {
            if (caseSensitive == true)
                return StringComparer.CurrentCulture;
            return StringComparer.CurrentCultureIgnoreCase;
        }

        public static int Ref
        {
            get { return StringResource.refCount; }
            set
            {
                StringResource.refCount = value;
                if (StringResource.refCount == 0)
                    StringResource.tableStrings.Clear();
            }
        }
    }
}
