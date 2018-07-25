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

using Ntreev.Crema.Reader.Binary;
using Ntreev.Crema.Reader.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Reader
{
    public class CremaReader
    {
        public static IDataSet Read(string ipAddress, int port)
        {
            return Read(ipAddress, port, "default");
        }

        public static IDataSet Read(string ipAddress, int port, string dataBase)
        {
            return Read(ipAddress, port, dataBase, "all");
        }

        public static IDataSet Read(string ipAddress, int port, string dataBase, string tags)
        {
            return Read(ipAddress, port, dataBase, tags, string.Empty);
        }

        public static IDataSet Read(string ipAddress, int port, string dataBase, string tags, string filterExpression)
        {
            return Read(ipAddress, port, dataBase, tags, filterExpression, false);
        }

        public static IDataSet Read(string ipAddress, int port, string dataBase, string tags, string filterExpression, bool isDevmode)
        {
            if (dataBase == null)
                throw new ArgumentNullException("database");
            if (tags == null)
                throw new ArgumentNullException("tags");
            if (filterExpression == null)
                throw new ArgumentNullException("filterExpression");
            var dic = new Dictionary<string, object>();

            dic.Add("type", "bin");
            dic.Add("tags", tags);
            dic.Add("database", dataBase);
            dic.Add("devmode", isDevmode);

            if (filterExpression != string.Empty)
                dic.Add("filter", filterExpression);

            var items = dic.Select(i => string.Format("{0}=\"{1}\"", i.Key, i.Value)).ToArray();
            var name = string.Join(";", items);

            using (var stream = new RemoteStream(ipAddress, port + 1, name))
            {
                return CremaReader.Read(stream);
            }
        }

        public static IDataSet Read(string filename)
        {
            return Read(filename, ReadOptions.None);
        }

        public static IDataSet Read(string filename, ReadOptions options)
        {
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return CremaReader.Read(stream, options);
        }

        public static IDataSet Read(Stream stream)
        {
            return Read(stream, ReadOptions.None);
        }

        public static IDataSet Read(Stream stream, ReadOptions options)
        {
            var reader = new BinaryReader(stream);
            var magicValue = reader.ReadUInt32();

            if (magicValue == FileHeader.defaultMagicValue)
            {
                var binaryReader = new CremaBinaryReader();
                binaryReader.Read(stream, options);
                return binaryReader;
            }

            throw new NotSupportedException("지원되지 않은 형식입니다.");
        }
    }
}
