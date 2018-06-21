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
using Ntreev.Library;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ntreev.Crema.ServiceModel
{
    /// <summary>
    /// mono 환경에서는 특히 서비스 계약중 Callback 메소드 인자로 
    /// 특이한 타입을 포함하는 DataMember가 있을 경우(예를들어 object[])
    /// 클라이언트 측에서 서비스 실행이 안되는 경우가 발생
    /// 따라서 string 타입의 Xml속성을 DataMember로 지정하고
    /// 나머지는 XmlSerializer를 이용해서 Xml을 생성함.
    /// 또한 FaultException 처리도 안되있어서 모든 서비스 계약에 예외가 발생하지 않는다는 전제하에
    /// 예외 정보를 담아서 전달함
    /// 문제는 XmlSerializer 기능이 DataContractSerializer보다 좋지 않아서 Serialize할 수 없는 타입이 존재함
    /// 예를들어 TimeSpan 타입
    /// </summary>
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct ResultBase
    {
        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public CremaFault Fault
        {
            get; set;
        }

        public void Validate()
        {
            if (this.Fault != null)
                throw new Exception(this.Fault.Message);
        }
    }

    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct ResultBase<T>
    {
        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public T Value { get; set; }

        [DataMember]
        public CremaFault Fault
        {
            get; set;
        }

        public void Validate()
        {
            if (this.Fault != null)
            {
                throw new Exception(this.Fault.Message);
            }
        }
    }

    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct ResultBase<T1, T2>
    {
        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public T1 Value1 { get; set; }

        [DataMember]
        public T2 Value2 { get; set; }

        [DataMember]
        public CremaFault Fault
        {
            get; set;
        }

        public void Validate()
        {
            if (this.Fault != null)
            {
                throw new Exception(this.Fault.Message);
            }
        }
    }
}
