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
using System.Runtime.InteropServices;
using System.Text;

namespace Ntreev.Crema.Runtime.Serialization.Binary
{
    public static class BinaryWriterExtension
    {
        public static long GetPosition(this BinaryWriter writer)
        {
            return writer.Seek(0, SeekOrigin.Current);
        }

        public static void SetPosition(this BinaryWriter writer, long pos)
        {
            writer.Seek((int)pos, SeekOrigin.Begin);
        }

        public static void WriteResourceString(this BinaryWriter writer, KeyValuePair<int, string> value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.Value);
            writer.WriteValue(value.Key);
            writer.WriteValue(bytes.Length);
            writer.Write(bytes, 0, bytes.Length);
        }

        public static void WriteResourceStrings(this BinaryWriter writer, KeyValuePair<int, string>[] strings)
        {
            writer.WriteValue(strings.Length);
            foreach (var item in strings)
            {
                writer.WriteResourceString(item);
            }
        }

        public static void WriteArray<T>(this BinaryWriter writer, T[] values)
            where T : struct
        {
            foreach (T value in values)
            {
                writer.WriteValue(value);
            }
        }

        public static void WriteValue<T>(this BinaryWriter writer, T value)
            where T : struct
        {
            var bytes = BinaryWriterExtension.GetBytes<T>(value);
            writer.Write(bytes, 0, bytes.Length);
        }

        public static byte[] GetBytes<TStruct>(TStruct data)
            where TStruct : struct
        {
            var structSize = Marshal.SizeOf(typeof(TStruct));
            var buffer = new byte[structSize];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(data, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return buffer;
        }
    }
}
