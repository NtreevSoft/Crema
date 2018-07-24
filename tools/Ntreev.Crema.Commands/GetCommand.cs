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

using Ntreev.Crema.Runtime.Generation;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands
{
    [Export(typeof(ICommand))]
    [CommandStaticProperty(typeof(CodeSettings))]
    [CommandStaticProperty(typeof(FilterSettings))]
    [CommandStaticProperty(typeof(DataBaseSettings))]
    class GetCommand : CommandBase
    {
        [Import]
        private IRuntimeService service = null;
        [ImportMany]
        private IEnumerable<ICodeGenerator> generators = null;
        [ImportMany]
        private IEnumerable<ICodeCompiler> compilers = null;
        [ImportMany]
        private IEnumerable<IDataSerializer> serializers = null;
        [Import]
        private Lazy<CommandContext> commandContext = null;

        public GetCommand()
            : base("get")
        {
        }

        [CommandProperty(IsRequired = true)]
        public string Address
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        public string OutputPath
        {
            get; set;
        }

        [CommandProperty]
        [DefaultValue("bin")]
        public string DataType
        {
            get; set;
        }

        [CommandProperty]
        [DefaultValue("crema.dat")]
        public string DataFilename
        {
            get; set;
        }

        [CommandProperty]
        public string Revision
        {
            get; set;
        }

        [CommandPropertyArray]
        public string[] Arguments
        {
            get; set;
        }

        [CommandProperty]
#if DEBUG
        [DefaultValue("en-US")]
#else
        [DefaultValue("")]
#endif
        public string Culture
        {
            get; set;
        }

        protected override void OnExecute()
        {
            if (this.Culture != string.Empty)
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(this.Culture);
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(this.Culture);
            }

            this.Out.WriteLine("receiving info");
            var metaData = this.service.GetMetaData(this.Address, DataBaseSettings.DataBaseName, DataBaseSettings.Tags, FilterSettings.FilterExpression, CodeSettings.Devmode, this.Revision);

            var generationSettings = new CodeGenerationSettings()
            {
                ClassName = CodeSettings.ClassName,
                Namespace = CodeSettings.Namespace,
                BaseNamespace = CodeSettings.BaseNamespace,
                Prefix = CodeSettings.Prefix,
                Postfix = CodeSettings.Postfix,
                BasePath = CodeSettings.BasePath,
                Options = CodeSettings.Options,
                Tags = (TagInfo)DataBaseSettings.Tags,
                Revision = this.Revision,
            };

            foreach (var item in CommandStringUtility.ArgumentsToDictionary(this.Arguments))
            {
                generationSettings.Arguments.Add(item);
            }

            if (CodeSettings.IsBuildMode == true)
            {
                var compiler = this.compilers.FirstOrDefault(item => item.Name == CodeSettings.LanguageType);
                if (compiler == null)
                    throw new InvalidOperationException($"'{CodeSettings.LanguageType}'은(는) 존재하지 언어입니다.");
                this.Out.WriteLine("compiling code.");
                compiler.Compile(this.OutputPath, metaData.Item1, generationSettings, CodeSettings.BuildTarget);
                this.Out.WriteLine("code compiled.");
            }
            else
            {
                this.Out.WriteLine("code generating.");
                var generator = this.generators.FirstOrDefault(item => item.Name == CodeSettings.LanguageType);
                if (generator == null)
                    throw new InvalidOperationException($"'{CodeSettings.LanguageType}'은(는) 존재하지 언어입니다.");
                generator.Generate(this.OutputPath, metaData.Item1, generationSettings);
                this.Out.WriteLine("code generated.");
            }

            this.Out.WriteLine("data serializing.");
            var serializer = this.serializers.FirstOrDefault(item => item.Name == this.DataType);
            serializer.Serialize(Path.Combine(this.OutputPath, this.DataFilename), metaData.Item2);
            this.Out.WriteLine("data serialized.");
        }

        private TextWriter Out
        {
            get { return this.commandContext.Value.Out; }
        }
    }
}
