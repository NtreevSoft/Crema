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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles.Properties
{
    [ResourceDescription("../Resources", IsShared = true)]
    public static class DataSetTypeProperties
    {
        [CommandProperty]
        [CommandPropertyTrigger(nameof(TypeOnly), false)]
        [DefaultValue(false)]
        public static bool TableOnly
        {
            get; set;
        }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(TableOnly), false)]
        [CommandPropertyTrigger(nameof(OmitContent), false)]
        [DefaultValue(false)]
        public static bool TypeOnly
        {
            get; set;
        }

        [CommandProperty]
        [DefaultValue(false)]
        [CommandPropertyTrigger(nameof(TypeOnly), false)]
        public static bool OmitContent
        {
            get; set;
        }

        public static DataSetType DataSetType
        {
            get
            {
                if (OmitContent == true)
                    return DataSetType.OmitContent;
                else if (TypeOnly == true)
                    return DataSetType.TypeOnly;
                return DataSetType.All;
            }
        }
    }
}
