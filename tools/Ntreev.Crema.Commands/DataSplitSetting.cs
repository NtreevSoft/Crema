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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Commands;

namespace Ntreev.Crema.Commands
{
    static class DataSplitSetting
    {
        private static string ext;

        [CommandProperty]
        [Description("크레마의 테이블 이름별로 데이터 파일을 출력합니다.\n이 옵션을 사용하면 필수인자 <filename> 은 출력할 디렉토리로 사용합니다.")]
        [DefaultValue(false)]
        public static bool Split
        {
            get; set;
        }

        [CommandProperty]
        [Description("--split 옵션을 사용할 경우 이 옵션으로 출력 파일의 확장자를 지정합니다.")]
        [DefaultValue("dat")]
        public static string Ext
        {
            get => ext;
            set => ext = value.StartsWith(".") ? value.Remove(0, 1) : value;
        }
    }
}
