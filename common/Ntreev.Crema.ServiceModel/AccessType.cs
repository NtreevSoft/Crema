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

using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceModel
{
    [Flags]
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public enum AccessType
    {
        /// <summary>
        /// 아무 권한 없음
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 보기 가능
        /// </summary>
        [EnumMember]
        Guest = 1,

        /// <summary>
        /// 테이블 내용 편집
        /// </summary>
        [EnumMember]
        Editor = 2 | Guest,

        /// <summary>
        /// 테이블 및 타입 형태 편집
        /// </summary>
        [EnumMember]
        Developer = 4 | Editor,

        /// <summary>
        /// 생성, 이름변경, 삭제, 이동
        /// </summary>
        [EnumMember]
        Master = 8 | Developer,

        /// <summary>
        /// 공유 설정 해제 및 Owner 위임
        /// </summary>
        [EnumMember]
        Owner = 16 | Master,

        /// <summary>
        /// 관리자가 잠금 기능을 설정한 후에 수행할 수 있는 특수 타입
        /// 설정이 불가능한 타입
        /// </summary>
        [EnumMember]
        [Browsable(false)]
        System = 32 | Owner,
    }
}
