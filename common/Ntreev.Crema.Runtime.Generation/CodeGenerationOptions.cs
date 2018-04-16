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

namespace Ntreev.Crema.Runtime.Generation
{
    [Flags]
    public enum CodeGenerationOptions
    {
        None = 0,

        /// <summary>
        /// 주석을 기재하지 않습니다.
        /// </summary>
        OmitComments = 1,

        /// <summary>
        /// 생성 날짜 또는 변경 날짜를 기재하지 않습니다.
        /// </summary>
        OmitSignatureDate = 2,

        /// <summary>
        /// 기본 코드를 생성하지 않습니다.
        /// </summary>
        OmitBaseCode = 4,

        /// <summary>
        /// 코드를 생성하지 않습니다.
        /// </summary>
        OmitCode = 8,

        /// <summary>
        /// 개발모드로 코드를 생성합니다.
        /// 개발모드로 설정되면 각 테이블 로드시 예외를 catch할 수있는 코드가 추가됩니다.
        /// </summary>
        Devmode = 16,
    }
}
