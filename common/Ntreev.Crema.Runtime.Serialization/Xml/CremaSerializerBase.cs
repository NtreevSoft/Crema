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

//using Ntreev.Library.IO;
//using Ntreev.Library;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Ntreev.Library.Linq;
//using Ntreev.Crema.Data;

//namespace Ntreev.Crema.RuntimeService.Xml
//{
//    public abstract class CremaSerializerBase
//    {
//        private readonly TimeManager tableTimes = new TimeManager("table");
//        private readonly TimeManager typeTimes = new TimeManager("type");
//        private readonly CremaDataSet dataSet;

//        public CremaSerializerBase(CremaDataSet dataSet)
//        {
//            this.dataSet = dataSet;
//        }

//        protected abstract void SerializeTable(Stream stream, CremaDataTable dataTable, long modifiedTime);

//        protected abstract void SerializeType(Stream stream, CremaDataType dataType, long modifiedTime);

//        protected abstract void Link(Stream stream, IEnumerable<string> files);

//        protected abstract string TableExtension
//        {
//            get;
//        }

//        protected abstract string TypeExtension
//        {
//            get;
//        }

//        private bool ShouldSerialize(string filename, TimeManager times, long modifiedTime)
//        {
//            if (File.Exists(filename) == false)
//                return true;

//            if (times.ContainsKey(filename) == false)
//                return true;

//            return times[filename] != modifiedTime;
//        }

//        private string[] SerializeCore(string intermediatePath, string tableName, long modifiedTime)
//        {
//            List<string> files = new List<string>(dataSet.Tables.Count);

//            var table = this.dataSet.Tables[tableName];
//            foreach (var item in EnumerableUtility.Friends(table, table.Childs))
//            {
//                string filename = this.GenerateFilename(intermediatePath, item.Name, this.TableExtension);

//                using (FileStream fs = new FileStream(filename, FileMode.Create))
//                {
//                    this.SerializeTable(fs, item, modifiedTime);
//                }

//                if (File.Exists(filename) == true)
//                    files.Add(filename);
//            }

//            return files.ToArray();
//        }

//        private void SerializeCore(string filename, CremaDataType type, long modifiedTime)
//        {
//            using (FileStream fs = new FileStream(filename, FileMode.Create))
//            {
//                this.SerializeType(fs, type, modifiedTime);
//            }
//        }

//        private string GenerateFilename(string intermediatePath, string itemName, string extension)
//        {
//            return Path.Combine(intermediatePath, itemName + extension);
//        }

//        private string[] GetFiles(string filename)
//        {
//            FileInfo fileInfo = new FileInfo(filename);
//            string searchPattern = string.Format("{0}.*{1}", Path.GetFileNameWithoutExtension(fileInfo.Name), fileInfo.Extension);

//            List<string> files = new List<string>();
//            files.Add(filename);
//            files.AddRange(fileInfo.Directory.GetFiles(searchPattern).Select(item => item.FullName));

//            return files.ToArray();
//        }

//        private void DeleteFiles(string filename)
//        {
//            foreach (var item in this.GetFiles(filename))
//            {
//                File.Delete(item);
//            }
//        }

//        #region IINISerializer

//        int ICrema2Serializer.SerializeTables(string intermediatePath, out string[] files)
//        {
//            DateTime time = DateTime.Now;

//            if (Directory.Exists(intermediatePath) == false)
//                Directory.CreateDirectory(intermediatePath);

//            var tableNames = this.dataSet.Tables.Where(item => item.Parent == null).Select(item => item.TableName).ToArray();

//            List<string> fileList = new List<string>(tableNames.Count());
//            int serialized = 0;
//            int index = 0;
//            int length = tableNames.Length;

//            this.tableTimes.Read(intermediatePath);

//#if DEBUG
//            foreach(var item in tableNames)
//#else
//            Parallel.ForEach(tableNames, item =>
//#endif
//            {
//                var table = this.dataSet.Tables[item];
//                long modifiedTime = table.ContentsInfo.DateTime.GetTotalSeconds();
//                int index2 = index++;
//                string[] fileNames = null;
//                string filename = this.GenerateFilename(intermediatePath, item, this.TableExtension);

//                if (this.ShouldSerialize(filename, this.tableTimes, modifiedTime) == true)
//                {
//                    this.DeleteFiles(filename);
//                    fileNames = this.SerializeCore(intermediatePath, item, modifiedTime);
//                    Trace.WriteLine(string.Format("{0}/{1} table is created : {2}", index2, length, item));
//                    serialized++;
//                }
//                else
//                {
//                    fileNames = this.GetFiles(filename);
//                }

//                lock (fileList)
//                {
//                    this.tableTimes.Set(filename, modifiedTime);
//                    fileList.AddRange(fileNames);
//                }
//#if DEBUG
//                }
//#else
//            });
//#endif
//            this.tableTimes.Write(intermediatePath);

//            Trace.WriteLine(string.Format("end : {0}", DateTime.Now - time));
//            files = fileList.ToArray();
//            return serialized;
//        }

//        int ICrema2Serializer.SerializeTypes(string intermediatePath, out string[] files)
//        {
//            DateTime time = DateTime.Now;

//            if (this.TypeExtension == null)
//            {
//                files = new string[0];
//                return 0;
//            }

//            if (Directory.Exists(intermediatePath) == false)
//                Directory.CreateDirectory(intermediatePath);

//            var typeNames = this.dataSet.Types.Select(item => item.TypeName).ToArray();

//            List<string> fileList = new List<string>(typeNames.Count());
//            int serialized = 0;
//            int index = 0;
//            int length = typeNames.Length;

//            this.typeTimes.Read(intermediatePath);

//            Parallel.ForEach(typeNames, item =>
//            {
//                CremaDataType type = this.dataSet.Types[item];
//                if (type == null)
//                    return;

//                long modifiedTime = type.ModificationInfo.DateTime.GetTotalSeconds();
//                int index2 = index++;
//                string filename = this.GenerateFilename(intermediatePath, item, this.TypeExtension);
//                if (this.ShouldSerialize(filename, this.typeTimes, modifiedTime) == true)
//                {
//                    this.SerializeCore(filename, type, modifiedTime);
//                    Trace.WriteLine(string.Format("{0}/{1} type is created : {2}", index2, length, item));
//                    serialized++;
//                }

//                lock (fileList)
//                {
//                    this.typeTimes.Set(filename, modifiedTime);
//                    fileList.Add(filename);
//                }
//            });

//            this.typeTimes.Write(intermediatePath);

//            Trace.WriteLine(string.Format("end : {0}", DateTime.Now - time));
//            files = fileList.ToArray();
//            return serialized;
//        }

//        long ICrema2Serializer.Link(string filename, IEnumerable<string> files)
//        {
//            FileInfo fileInfo = new FileInfo(filename);

//            if(fileInfo.Exists == true)
//                fileInfo.Delete();

//            DirectoryUtility.PrepareFile(filename);

//            using (FileStream fs = fileInfo.OpenWrite())
//            {
//                this.Link(fs, files);
//            }

//            fileInfo.Refresh();
//            return fileInfo.GetModifyTime();
//        }

//        #endregion

//        #region classes

//        class TimeManager : Dictionary<string, long>
//        {
//            private readonly string name;

//            public TimeManager(string name)
//            {
//                this.name = name;
//            }

//            public void Read(string intermediatePath)
//            {
//                string filename = this.GenerateFileName(intermediatePath);

//                if (File.Exists(filename) == false)
//                    return;

//                this.Clear();

//                foreach (var item in File.ReadLines(filename))
//                {
//                    string[] ss = item.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries);

//                    try
//                    {
//                        this.Add(ss[0], long.Parse(ss[1]));
//                    }
//                    catch (Exception)
//                    {
//                        continue;
//                    }
//                }
//            }

//            public void Write(string intermediatePath)
//            {
//                string filename = this.GenerateFileName(intermediatePath);

//                if (File.Exists(filename) == true)
//                    File.Delete(filename);


//                var query = from item in this
//                            select string.Format("{0}, {1}", item.Key, item.Value);

//                File.WriteAllLines(filename, query);
//            }

//            private string GenerateFileName(string intermediatePath)
//            {
//                return Path.Combine(intermediatePath, name + ".tim");
//            }

//            public void Set(string item, long modifiedTime)
//            {
//                if (this.ContainsKey(item) == false)
//                    this.Add(item, modifiedTime);
//                else
//                    this[item] = modifiedTime;
//            }
//        }

//        #endregion
//    }
//}
