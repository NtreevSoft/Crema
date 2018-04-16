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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ntreev.Crema.Reader.IO
{
    static class BinaryReaderExtension
    {
        public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin)
        {
            return reader.BaseStream.Seek(offset, origin);
        }

        public static T[] ReadValues<T>(this BinaryReader reader, int count)
            where T : struct
        {
            List<T> list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                T value;
                reader.ReadValue(out value);
                list.Add(value);
            }
            return list.ToArray();
        }

#if UNITY_WEBPLAYER
        public static void ReadValue<T>(this BinaryReader reader, out T value)
            where T : struct
        {
            int size = SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            int readSize = reader.Read(bytes, 0, size);
            if (readSize != size)
                throw new Exception();
            PtrToStructure(bytes, out value);
        }

         private static void PtrToStructure<T>(byte[] bytes, out T ptr)
             where T : struct
        {
            ptr = new T();
            object box = ptr;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(ptr);

            var query = from PropertyDescriptor item in TypeDescriptor.GetProperties(ptr)
                        select SizeOfPrimitiveType(item.PropertyType);

            int maxSizeOfType = query.Max();

            int offset = 0;
            foreach (PropertyDescriptor prop in props)
            {

                int typeSize = SizeOfPrimitiveType(prop.PropertyType);
                Type type = prop.PropertyType;

                if (offset + typeSize > maxSizeOfType)
                {
                    offset += (offset % maxSizeOfType);
                }

                if (type == typeof(bool))
                    prop.SetValue(box, BitConverter.ToBoolean(bytes, offset));
                else if (type == typeof(char))
                    prop.SetValue(box, BitConverter.ToChar(bytes, offset));
                else if (type == typeof(byte))
                    prop.SetValue(box, BitConverter.ToChar(bytes, offset));
                else if (type == typeof(sbyte))
                    prop.SetValue(box, BitConverter.ToChar(bytes, offset));
                else if (type == typeof(short))
                    prop.SetValue(box, BitConverter.ToInt16(bytes, offset));
                else if (type == typeof(ushort))
                    prop.SetValue(box, BitConverter.ToUInt16(bytes, offset));
                else if (type == typeof(int))
                    prop.SetValue(box, BitConverter.ToInt32(bytes, offset));
                else if (type == typeof(uint))
                    prop.SetValue(box, BitConverter.ToUInt32(bytes, offset));
                else if (type == typeof(long))
                    prop.SetValue(box, BitConverter.ToInt64(bytes, offset));
                else if (type == typeof(ulong))
                    prop.SetValue(box, BitConverter.ToUInt64(bytes, offset));
                else if (type == typeof(float))
                    prop.SetValue(box, BitConverter.ToSingle(bytes, offset));
                else if (type == typeof(double))
                    prop.SetValue(box, BitConverter.ToDouble(bytes, offset));

                offset += typeSize;
            }

            ptr = (T)box;
        }

        private static int SizeOf(Type type)
        {
            var query = from PropertyDescriptor item in TypeDescriptor.GetProperties(type)
                        select SizeOfPrimitiveType(item.PropertyType);

            int maxSizeOfType = query.Max();

            int size = 0;
            foreach (var item in query)
            {
                if (size + item > maxSizeOfType)
                {
                    size += (size % maxSizeOfType);
                }
                size += item;
            }

            return size;
        }

        private static int SizeOfPrimitiveType(Type type)
        {
            if (type == typeof(bool))
                return sizeof(bool);
            else if (type == typeof(char))
                return sizeof(char);
            else if (type == typeof(byte))
                return sizeof(byte);
            else if (type == typeof(sbyte))
                return sizeof(sbyte);
            else if (type == typeof(short))
                return sizeof(short);
            else if (type == typeof(ushort))
                return sizeof(ushort);
            else if (type == typeof(int))
                return sizeof(int);
            else if (type == typeof(uint))
                return sizeof(uint);
            else if (type == typeof(long))
                return sizeof(long);
            else if (type == typeof(ulong))
                return sizeof(ulong);
            else if (type == typeof(float))
                return sizeof(float);
            else if (type == typeof(double))
                return sizeof(double);

            throw new NotSupportedException();
        }
#else
        public static void ReadValue<T>(this BinaryReader reader, out T value)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            int readSize = reader.Read(bytes, 0, size);
            if (readSize != size)
                throw new Exception();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            value = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
        }

        public static T ReadValue<T>(this BinaryReader reader)
            where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            int readSize = reader.Read(bytes, 0, size);
            if (readSize != size)
                throw new Exception();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            T value = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return value;
        }
#endif
    }
}
