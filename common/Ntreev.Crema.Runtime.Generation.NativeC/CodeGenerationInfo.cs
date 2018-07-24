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
using Ntreev.Library;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Runtime.Generation.NativeC
{
    public class CodeGenerationInfo
    {
        //public const string ReaderNamespace = "CremaReader";
        public const string DefaultNamespace = "CremaCode";
        public const string DefaultClassName = "CremaDataSet";

        public const string CremaReaderName = "CremaReader";
        public const string CremaDataName = "CremaData";
        public const string CremaTableName = "CremaTable";
        public const string CremaRowName = "CremaRow";

        private readonly GenerationSet metaData;
        private readonly CodeGenerationSettings settings;

        public CodeGenerationInfo(GenerationSet metaData, CodeGenerationSettings settings)
        {
            this.metaData = metaData;
            this.settings = settings;
            this.EnumFomrat = (v) => $"0x{(int)v:x8}";
        }

        public IEnumerable<TableInfo> GetTables()
        {
            return GetTables(false);
        }

        public IEnumerable<TableInfo> GetTables(bool includeDerived)
        {
            foreach (var item in this.Tables)
            {
                //if (item.InheritedTags.CanUse(this.Tags) == false)
                //    continue;

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
                    return DefaultNamespace;
                return this.settings.BaseNamespace;
            }
        }

        public string ReaderNamespace
        {
            get
            {
                return this.BaseNamespace + "::" + "reader";
            }
        }

        public string Prefix
        {
            get
            {
                if (this.settings.Prefix == string.Empty)
                    return "crema_";
                return this.settings.Prefix;
            }
        }

        public string Postfix
        {
            get { return this.settings.Postfix; }
        }

        public bool NoComment
        {
            get
            {
                return this.settings.Options.HasFlag(CodeGenerationOptions.OmitComments);
            }
        }

        public bool NoChanges
        {
            get
            {
                return this.settings.Options.HasFlag(CodeGenerationOptions.OmitSignatureDate);
            }
        }

        public bool IsDevmode
        {
            get { return this.settings.Options.HasFlag(CodeGenerationOptions.Devmode); }
        }

        public bool BlankLinesBetweenMembers
        {
            get;
            set;
        }

        public string RelativePath
        {
            get; set;
        }

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

        [Obsolete("타입 출력시 값의 포맷을 지정하는 임시 변수")]
        public Func<long, string> EnumFomrat
        {
            get; set;
        }
    }
}
