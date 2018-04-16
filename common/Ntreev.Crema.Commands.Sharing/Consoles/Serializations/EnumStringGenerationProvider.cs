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

using Newtonsoft.Json.Schema.Generation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace Ntreev.Crema.Commands.Consoles.Serializations
{
    class EnumStringGenerationProvider : JSchemaGenerationProvider
    {
        private readonly string propertyName;

        public EnumStringGenerationProvider(params object[] args)
        {
                this.propertyName = (string)args.FirstOrDefault();
            //this.propertyName = propertyName;
        }

        public override JSchema GetSchema(JSchemaTypeGenerationContext context)
        {
            var definedType = context.ParentContract.CreatedType;
            var schema = JsonSchemaUtility.GetSchema(context.ObjectType);

            var propertyInfo = definedType.GetProperty(this.propertyName);
            var items = propertyInfo.GetValue(null) as string[];

            if (items != null)
            {
                schema.Enum.Clear();
                foreach (var item in items)
                {
                    schema.Enum.Add(JValue.CreateString(item));
                }
            }


            //int qwr = 0;

            //bool isNullable = ReflectionUtils.IsNullableType(context.ObjectType);
            //Type t = context.ObjectType;

            //var schema = new JSchema
            //{
            //    Title = context.SchemaTitle,
            //    Description = context.SchemaDescription,
            //    Type = JSchemaType.String
            //};

            //context.g

            //if (isNullable && context.Required != Required.Always && context.Required != Required.DisallowNull)
            //{
            //    schema.Type |= JSchemaType.Null;
            //    schema.Enum.Add(JValue.CreateNull());
            //}

            //string[] names = Enum.GetNames(t);

            //foreach (string name in names)
            //{
            //    string finalName = EnumUtils.ToEnumName(t, name, CamelCaseText);

            //    schema.Enum.Add(JValue.CreateString(finalName));
            //}

            return schema;
        }
    }
}
