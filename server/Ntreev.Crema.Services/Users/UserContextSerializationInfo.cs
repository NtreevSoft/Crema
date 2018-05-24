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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Users
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct UserContextSerializationInfo
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public UserCategorySerializationInfoList CategoriesList { get; set; }

        [DataMember]
        public UserSerializationInfo[] Users { get; set; }

        [IgnoreDataMember]
        public string[] Categories
        {
            get { return this.CategoriesList?.ToArray(); }
            set
            {
                if (value == null)
                {
                    this.CategoriesList = null;
                }
                else 
                {
                    if (this.CategoriesList == null)
                        this.CategoriesList = new UserCategorySerializationInfoList();
                    else
                        this.CategoriesList.Clear();
                    this.CategoriesList.AddRange(value);
                }
            }
        }
    }
}
