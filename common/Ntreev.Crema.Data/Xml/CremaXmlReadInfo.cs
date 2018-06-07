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

using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Schema;

namespace Ntreev.Crema.Data.Xml
{
    public class CremaXmlReadInfo
    {
        private readonly string xmlPath;
        private readonly string xsdPath;
        private readonly string relativeXsdPath;
        private readonly ItemName itemName;
        private readonly long xmlSize;
        private readonly bool isInherited;

        private Version version = new Version(2, 0);

        public CremaXmlReadInfo(string xmlPath)
        {
            if (FileUtility.IsAbsolute(xmlPath) == false)
                throw new ArgumentException("\"${xmlPath}\" 은(는) 절대 경로가 아닙니다.", nameof(xmlPath));
            if (File.Exists(xmlPath) == false)
                throw new FileNotFoundException($"\"{xmlPath}\" 경로를 찾을 수 없습니다.");

            this.xmlPath = xmlPath;
            this.xsdPath = Path.ChangeExtension(xmlPath, CremaSchema.SchemaExtension);
            this.xmlSize = new FileInfo(xmlPath).Length;

            using (var reader = XmlReader.Create(this.xmlPath))
            {
                reader.MoveToContent();
                var text = reader.GetAttribute(CremaSchema.Version);
                if (string.IsNullOrEmpty(text) == false)
                {
                    Version.TryParse(text, out this.version);
                }
            }

            if (File.Exists(this.xsdPath) == false)
            {
                using (var reader = XmlReader.Create(xmlPath))
                {
                    reader.MoveToContent();
                    if (Version.TryParse(reader.GetAttribute(CremaSchema.Version), out Version version) == false)
                    {
                        version = new Version(2, 0);
                    }

                    if (version.Major >= 3)
                    {
                        FindSchemaLocation(reader, xmlPath, out this.xsdPath, out string tableNamespace);
                        var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchema.TableNamespace, tableNamespace);
                        var tableName = CremaDataSet.GetTableName(CremaSchema.TableNamespace, tableNamespace);
                        this.itemName = new ItemName(categoryPath, tableName);
                    }
                    else
                    {
                        FindSchemaLocationVersion2(reader, xmlPath, out this.xsdPath, out string tableNamespace);
                        var categoryPath = CremaDataSet.GetTableCategoryPath(CremaSchemaObsolete.TableNamespaceObsolete, tableNamespace);
                        var tableName = CremaDataSet.GetTableName(CremaSchemaObsolete.TableNamespaceObsolete, tableNamespace);
                        this.itemName = new ItemName(categoryPath, tableName);
                    }
                }
                this.isInherited = true;
            }

            this.relativeXsdPath = UriUtility.MakeRelative(this.xmlPath, this.xsdPath);
        }

        public string[] GetTypePaths()
        {
            var prefix = "xs";
            var schemaDirectory = System.IO.Path.GetDirectoryName(this.SchemaPath);
            var namespaceResolver = new XmlNamespaceManager(new NameTable());
            namespaceResolver.AddNamespace(prefix, XmlSchema.Namespace);
            var doc = XDocument.Load(this.SchemaPath);
            var query = from item in doc.XPathSelectElements($"/{prefix}:schema/{prefix}:import", namespaceResolver)
                        let schemaLocation = item.Attribute(XName.Get("schemaLocation")).Value
                        let uri = new Uri(UriUtility.Combine(schemaDirectory, schemaLocation))
                        select uri.LocalPath;

            return query.ToArray();
        }

        public string XmlPath
        {
            get { return this.xmlPath; }
        }

        public string SchemaPath
        {
            get { return this.xsdPath; }
        }

        public string RelativeSchemaPath
        {
            get { return this.relativeXsdPath; }
        }

        public ItemName ItemName
        {
            get { return this.itemName; }
        }

        public bool IsInherited
        {
            get { return this.isInherited; }
        }

        public long XmlSize
        {
            get { return this.xmlSize; }
        }

        public Version Version
        {
            get { return this.version; }
        }

        private static void FindSchemaLocation(XmlReader reader, string xmlPath, out string xsdPath, out string tableNamespace)
        {
            var value = reader.GetAttribute("schemaLocation", XmlSchema.InstanceNamespace);
            var ss = value.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            if (ss.Length != 2)
                throw new CremaDataException();

            tableNamespace = ss[0];
            xsdPath = ss[1];

            if (FileUtility.IsAbsolute(xsdPath) == false)
            {
                xsdPath = new Uri(new Uri(xmlPath), xsdPath).LocalPath;
            }
        }

        private static void FindSchemaLocationVersion2(XmlReader reader, string xmlPath, out string xsdPath, out string tableNamespace)
        {
            var targetNamespace = reader.NamespaceURI;
            var relativeNamespace = UriUtility.MakeRelativeOfDirectory(CremaSchemaObsolete.TableNamespaceObsolete, targetNamespace);
            var relativeXsdPath = PathUtility.ConvertFromUri(relativeNamespace + CremaSchema.SchemaExtension);
            var path = xmlPath;
            var index = 0;
            xsdPath = null;
            while ((index = path.LastIndexOf(Path.DirectorySeparatorChar + CremaSchemaObsolete.TableDirectoryObsolete + Path.DirectorySeparatorChar)) >= 0)
            {
                var targetPath = Path.Combine(xmlPath.Remove(index), CremaSchemaObsolete.TableDirectoryObsolete);
                var xsdFilename = Path.GetFileName(relativeXsdPath);
                var found = Directory.GetFiles(targetPath, xsdFilename, SearchOption.AllDirectories).Where(item => item.EndsWith(relativeXsdPath)).FirstOrDefault();
                if (File.Exists(found) == true)
                {
                    xsdPath = found;
                    break;
                }
                path = path.Remove(index);
            }

            if (xsdPath == null)
                throw new Exception();

            var basePath = xsdPath.Replace(relativeXsdPath, string.Empty);
            var relative = FileUtility.RemoveExtension(xmlPath.Replace(basePath, string.Empty));
            var uri = new Uri(new Uri(CremaSchemaObsolete.TableNamespaceObsolete + Path.AltDirectorySeparatorChar), relative);
            tableNamespace = uri.ToString();
        }
    }
}
