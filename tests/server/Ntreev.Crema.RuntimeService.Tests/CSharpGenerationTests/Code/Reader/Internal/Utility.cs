﻿//Released under the MIT License.
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

namespace Ntreev.Crema.Code.Reader.Internal
{
    static class Utility
    {
        public static Type NameToType(string typeName)
        {
            if (typeName == "boolean")
                return typeof(bool);
            else if (typeName == "string")
                return typeof(string);
            else if (typeName == "float")
                return typeof(float);
            else if (typeName == "double")
                return typeof(double);
            else if (typeName == "byte")
                return typeof(sbyte);
            else if (typeName == "unsignedByte")
                return typeof(byte);
            else if (typeName == "short")
                return typeof(short);
            else if (typeName == "unsignedShort")
                return typeof(ushort);
            else if (typeName == "int")
                return typeof(int);
            else if (typeName == "unsignedInt")
                return typeof(uint);
            else if (typeName == "long")
                return typeof(long);
            else if (typeName == "unsignedLong")
                return typeof(ulong);
            else if (typeName == "dateTime")
                return typeof(DateTime);
            else if (typeName == "duration")
                return typeof(TimeSpan);

            return typeof(int);
        }
    }
}
