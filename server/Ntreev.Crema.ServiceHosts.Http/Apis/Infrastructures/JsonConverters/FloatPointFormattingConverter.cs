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
using Newtonsoft.Json;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.JsonConverters
{
    internal class FloatPointFormattingConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is float || value is double)
            {
                var doubleVal = double.Parse(value.ToString());
                if (doubleVal.ToString() == Math.Truncate(doubleVal).ToString())
                {
                    writer.WriteRawValue(Math.Truncate(doubleVal).ToString());
                    return;
                }
            }
            else if (value is decimal)
            {
                var decimalVal = decimal.Parse(value.ToString());
                if (decimalVal.ToString() == Math.Truncate(decimalVal).ToString())
                {
                    writer.WriteRawValue(Math.Truncate(decimalVal).ToString());
                    return;
                }
            }

            writer.WriteRawValue(JsonConvert.ToString(value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(float) || objectType == typeof(double) || objectType == typeof(decimal);
        }
    }
}
