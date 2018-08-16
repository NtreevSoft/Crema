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

namespace Ntreev.Crema.Runtime.Serialization.Binary
{
    public struct BinaryTableHeader
    {
        public const int DefaultMagicValue = 0x04000000;

        public int MagicValue { get; set; }

        public int HashValue { get; set; }

        [Obsolete("같은 리비전의 데이터를 출력해도 이 값이 변경되면 데이터 전체가 다르게 출력이 되기 때문에 사용을 하지 않아야 한다.")]
        public long ModifiedTime { get; set; }

        public long TableInfoOffset { get; set; }

        public long ColumnsOffset { get; set; }

        public long RowsOffset { get; set; }

        public long StringResourcesOffset { get; set; }

        public long UserOffset { get; set; }
    }
}
