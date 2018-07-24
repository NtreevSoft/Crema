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
using Ntreev.Crema.Runtime.Generation;
using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.TypeScript
{
    class CodeGenerationInfo
    {
        public const string DefaultBaseNamespace = "crema-code";
        public const string DefaultNamespace = "crema-code";
        public const string DefaultClassName = "CremaDataSet";

        private readonly GenerationSet metaData;
        private readonly CodeGenerationSettings settings;

        public CodeGenerationInfo(string outputPath, GenerationSet metaData, CodeGenerationSettings settings)
        {
            this.metaData = metaData;
            this.settings = settings;

            if (this.settings.BasePath != string.Empty)
            {
                var relativePath = UriUtility.MakeRelativeOfDirectory(new DirectoryInfo(outputPath).FullName, DirectoryUtility.GetAbsolutePath(outputPath, settings.BasePath));
                this.RelativePath = relativePath + Path.AltDirectorySeparatorChar;
            }
        }

        public IEnumerable<TableInfo> GetTables()
        {
            return GetTables(false);
        }

        public IEnumerable<TableInfo> GetTables(bool includeDerived)
        {
            foreach (var item in this.Tables)
            {
                if (includeDerived == false && item.TemplatedParent != string.Empty)
                    continue;

                if (item.ParentName != string.Empty)
                    continue;

                yield return item;
            }
        }

        public IEnumerable<TableInfo> GetChilds(TableInfo tableInfo)
        {
            foreach (var item in this.Tables)
            {
                if (item.ParentName == tableInfo.Name)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<TableInfo> Tables
        {
            get
            {
                foreach (var item in this.metaData.Tables)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<TypeInfo> Types
        {
            get
            {
                foreach (var item in this.metaData.Types)
                {
                    yield return item;
                }
            }
        }

        public string Revision
        {
            get { return this.metaData.Revision; }
        }

        public string RequestedRevision
        {
            get { return this.settings.Revision; }
        }

        public string DataBaseName
        {
            get { return this.metaData.Name; }
        }

        public string ClassName
        {
            get
            {
                if (this.settings.ClassName == string.Empty)
                    return DefaultClassName;
                return this.settings.ClassName;
            }
        }

        public string Namespace
        {
            get
            {
                if (this.settings.Namespace == string.Empty)
                    return DefaultNamespace;
                return this.settings.Namespace;
            }
        }

        public string BaseNamespace
        {
            get
            {
                if (this.settings.BaseNamespace == string.Empty)
                    return DefaultBaseNamespace;
                return this.settings.BaseNamespace;
            }
        }

        //public string ReaderNamespace
        //{
        //    get
        //    {
        //        return this.BaseNamespace + "::" + "reader";
        //    }
        //}

        public string Prefix
        {
            get { return this.settings.Prefix; }
        }

        public string Postfix
        {
            get { return this.settings.Postfix; }
        }

        public bool OmitComment
        {
            get
            {
                return this.settings.Options.HasFlag(CodeGenerationOptions.OmitComments);
            }
        }

        public bool OmitSignatureDate
        {
            get
            {
                return this.settings.Options.HasFlag(CodeGenerationOptions.OmitSignatureDate);
            }
        }

        public bool OmitDeclaration
        {
            get
            {
                if (this.settings.Arguments.ContainsKey("omitDecl") == true)
                {
                    if (this.settings.Arguments["omitDecl"] is bool b && b == true)
                        return true;
                }

                return false;
            }
        }

        public string TSLintDisable
        {
            get
            {
                if (this.settings.Arguments.ContainsKey("tslint:disable") == true)
                {
                    if (this.settings.Arguments["tslint:disable"] is string s)
                    {
                        return s;
                    }
                }

                return null;
            }
        }

        public bool IsDevmode
        {
            get { return this.settings.Options.HasFlag(CodeGenerationOptions.Devmode); }
        }

        public string RelativePath { get; private set; }

        public string TypesHashValue
        {
            get { return this.metaData.TypesHashValue; }
        }

        public string TablesHashValue
        {
            get { return this.metaData.TablesHashValue; }
        }

        public TagInfo Tags
        {
            get { return (TagInfo)this.metaData.Tags; }
        }
    }
}
