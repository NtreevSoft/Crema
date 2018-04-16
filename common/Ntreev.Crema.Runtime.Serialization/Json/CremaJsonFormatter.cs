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

//using Ntreev.Crema.Data.Xml;
//using Ntreev.Library.IO;
//using Ntreev.Crema.ServiceModel;
//using Ntreev.Crema.Data.Xml.Schema;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Xml;
//using System.Data;
//using System.ComponentModel;
//using Newtonsoft.Json;
//using Ntreev.Crema.Data;

//namespace Ntreev.Crema.RuntimeService.Json
//{
//    public class CremaJsonFormatter : IFormatter
//    {
//        private SerializationBinder binder;
//        private StreamingContext context;
//        private ISurrogateSelector surrogateSelector;

//        public CremaJsonFormatter()
//        {

//        }

//        public CremaDataSet Deserialize(Stream stream)
//        {
//            throw new NotImplementedException();
//        }

//        public void Serialize(Stream stream, CremaDataTable dataTable)
//        {
//            this.SerializeCore(stream, dataTable);
//        }

//        private void SerializeCore(Stream stream, CremaDataTable dataTable)
//        {
//            using (StreamWriter sw = new StreamWriter(stream))
//            using (JsonTextWriter writer = new JsonTextWriter(sw))
//            {
//                //writer.Formatting = Newtonsoft.Json.Formatting.Indented;
//                //writer.Indentation = 4;

//                var query = from item in dataTable.Columns
//                            select item;

//                writer.WriteStartObject();

//                writer.WritePropertyName("tableName");
//                writer.WriteValue(dataTable.TableName);
//                writer.WritePropertyName("categoryName");
//                writer.WriteValue(dataTable.CategoryName);
//                writer.WritePropertyName("columnCount");
//                writer.WriteValue(query.Count());
//                writer.WritePropertyName("rowCount");
//                writer.WriteValue(dataTable.Rows.Count);

//                writer.WritePropertyName("columns");
//                writer.WriteStartArray();

//                foreach (var item in query)
//                {
//                    writer.WriteStartObject();
//                    writer.WritePropertyName("columnName");
//                    writer.WriteValue(item.ColumnName);
//                    writer.WritePropertyName("dataType");
//                    writer.WriteValue(item.DataType.GetTypeName());
//                    writer.WritePropertyName("isKey");
//                    writer.WriteValue(item.IsKey);
//                    writer.WriteEndObject();
//                }

//                writer.WriteEndArray();

//                writer.WritePropertyName("rows");

//                this.WriteRows(writer, dataTable.Rows, query);


//                writer.WriteEndObject();
//            }
//        }

//        private void WriteRows(JsonTextWriter writer, IEnumerable<CremaDataRow> dataRows, IEnumerable<CremaDataColumn> dataColumns)
//        {
//            writer.WriteStartArray();
//            foreach (var item in dataRows)
//            {
//                WriteRow(writer, item, dataColumns);
//            }
//            writer.WriteEndArray();
//        }

//        private void WriteRow(JsonTextWriter writer, CremaDataRow dataRow, IEnumerable<CremaDataColumn> dataColumns)
//        {
//            writer.WriteStartObject();

//            foreach (var item in dataColumns)
//            {
//                object value = dataRow[item];
//                if (value == DBNull.Value)
//                    continue;

//                if (item.DataType.IsEnum == true)
//                {
//                    value = (int)Enum.Parse(item.DataType, value as string);
//                }

//                writer.WritePropertyName(item.ColumnName);
//                writer.WriteValue(value);
//            }
//            writer.WriteEndObject();
//        }

//        private string GetValue(CremaDataRow dataRow, CremaDataColumn dataColumn)
//        {
//            object value = dataRow[dataColumn];
//            if (value == DBNull.Value)
//                return string.Empty;

//            if (dataColumn.DataType.IsEnum == true)
//            {
//                value = (int)Enum.Parse(dataColumn.DataType, value as string);
//            }
//            else if (dataColumn.DataType == typeof(string))
//            {
//                value = "\"" + value.ToString() + "\"";
//            }
//            return value.ToString();
//        }

//        private IEnumerable<DataRelation> GetRelations(DataColumn dataColumn)
//        {
//            foreach (DataRelation item in dataColumn.Table.ChildRelations)
//            {
//                foreach (var column in item.ParentColumns)
//                {
//                    if (dataColumn == column)
//                        yield return item;
//                }
//            }
//        }

//        private string GetKey(DataRow dataRow)
//        {
//            var query = from item in dataRow.Table.PrimaryKey
//                        select dataRow[item].ToString();

//            return string.Join("_", query);
//        }


//        #region IFormatter

//        SerializationBinder IFormatter.Binder
//        {
//            get { return this.binder; }
//            set { this.binder = value; }
//        }

//        StreamingContext IFormatter.Context
//        {
//            get { return this.context; }
//            set { this.context = value; }
//        }

//        object IFormatter.Deserialize(Stream serializationStream)
//        {
//            throw new NotImplementedException();
//        }

//        void IFormatter.Serialize(Stream serializationStream, object graph)
//        {
//            this.Serialize(serializationStream, graph as CremaDataTable);
//        }

//        ISurrogateSelector IFormatter.SurrogateSelector
//        {
//            get { return this.surrogateSelector; }
//            set { this.surrogateSelector = value; }
//        }

//        #endregion
//    }
//}
