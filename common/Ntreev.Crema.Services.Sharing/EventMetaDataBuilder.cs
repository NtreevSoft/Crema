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

using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    static class EventMetaDataBuilder
    {
        public static object[] Build(object[] items, AccessChangeType changeType)
        {
            return Build(items, changeType, items.Select(item => string.Empty).ToArray(), items.Select(item => AccessType.None).ToArray());
        }

        public static object[] Build(object[] items, AccessChangeType changeType, string[] memberIDs)
        {
            return Build(items, changeType, memberIDs, items.Select(item => AccessType.None).ToArray());
        }

        public static object[] Build(object[] items, AccessChangeType changeType, string[] memberIDs, AccessType[] accessTypes)
        {
            return new object[] { changeType, memberIDs, accessTypes };
        }

        public static object[] Build(object[] items, LockChangeType changeType)
        {
            return Build(items, changeType, items.Select(item => string.Empty).ToArray());
        }

        public static object[] Build(object[] items, LockChangeType changeType, string[] comments)
        {
            return new object[] { changeType, comments };
        }

        public static object[] Build(object[] items, BanChangeType changeType)
        {
            return Build(items, changeType, items.Select(item => string.Empty).ToArray());
        }

        public static object[] Build(object[] items, BanChangeType changeType, string[] comments)
        {
            return new object[] { changeType , comments };
        }
    }
}