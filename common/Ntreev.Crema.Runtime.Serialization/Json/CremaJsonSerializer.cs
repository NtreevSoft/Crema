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

//#define _PARALLEL

//using Ntreev.Crema.Data;
//using Ntreev.Crema.RuntimeService.Binary;
//using Ntreev.Crema.ServiceModel;
//using Ntreev.Crema.Data.Xml;
//using Ntreev.Library.IO;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;


//namespace Ntreev.Crema.RuntimeService.Json
//{
//    public class CremaJsonSerializer
//    {
//        public const uint magicValue = 0x8d31269e;
//        private const string fileExtension = ".json";

//        private readonly HashSet<string> strings = new HashSet<string>();

//        public CremaJsonSerializer(CremaDataSet dataSet)
//        {

//        }

//        protected override string TableExtension
//        {
//            get { return CremaJsonSerializer.fileExtension; }
//        }

//        protected override string TypeExtension
//        {
//            get { return null; }
//        }

//        protected override void SerializeTable(Stream stream, CremaDataTable dataTable, long modifiedTime)
//        {
//            CremaJsonFormatter formatter = new CremaJsonFormatter();
//            formatter.Serialize(stream, dataTable);
//        }

//        protected override void SerializeType(Stream stream, CremaDataType dataType, long modifiedTime)
//        {
//            //throw new NotImplementedException();
//        }

//        protected override void Link(Stream stream, IEnumerable<string> files)
//        {
//#if DEBUG
//            StringBuilder sb = new StringBuilder();
//            using (StringWriter sw = new StringWriter(sb))
//#else
//            using(StreamWriter sw = new StreamWriter(stream))
//#endif
//            using (JsonTextWriter writer = new JsonTextWriter(sw))
//            {
//                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
//                writer.Indentation = 4;

//                writer.WriteComment("crema-data");
//                writer.WriteWhitespace(" ");

//                writer.WriteStartObject();
//                writer.WritePropertyName("tableCount");
//                writer.WriteValue(files.Count());

//                writer.WritePropertyName("tables");
//                writer.WriteStartArray();

//                foreach (var item in files)
//                {
//                    writer.WriteRawValue(File.ReadAllText(item));
//                }

//                writer.WriteEndArray();
//            }

//#if DEBUG
//            var parsedJson = JsonConvert.DeserializeObject(sb.ToString());
//            string text = JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
//            using (StreamWriter sw = new StreamWriter(stream))
//            {
//                sw.WriteLine(text);
//            }
//#endif
//            Trace.WriteLine("완료했습니다.");
//        }

//        private int GetStringID(string text)
//        {
//            text = text.Replace(Environment.NewLine, "\n");
//            if (this.strings.Contains(text) == false)
//                this.strings.Add(text);
//            return text.GetHashCode();
//        }

//        #region classes

//        public struct FileHeader
//        {
//            public uint MagicValue { get; set; }
//            public int Version { get; set; }
//            public long LinkTime { get; set; }
//            public int TableCount { get; set; }
//            public long IndexOffset { get; set; }
//            public long TablesOffset { get; set; }
//            public long StringResourcesOffset { get; set; }
//        }

//        public struct TableIndex
//        {
//            public int TableName { get; set; }
//            public long Offset { get; set; }
//        }

//        #endregion
//    }
//}
