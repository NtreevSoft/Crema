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

using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands
{
    [Export(typeof(ICommand))]
    [CommandStaticProperty(typeof(FilterSettings))]
    [CommandStaticProperty(typeof(DataBaseSettings))]
    [CommandStaticProperty(typeof(DataSplitSetting))]
    class GetDataCommand : CommandBase
    {
        [Import]
        private IRuntimeService service = null;
        [ImportMany]
        private IEnumerable<IDataSerializer> serializers = null;
        [Import]
        private Lazy<CommandContext> commandContext = null;

        public GetDataCommand()
            : base("get-data")
        {

        }

        [CommandProperty(IsRequired = true)]
        public string Address
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        public string Filename
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("bin")]
        public string OutputType
        {
            get; set;
        }

        [CommandProperty]
        [Description("개발 전용으로 생성합니다.")]
        public bool Devmode
        {
            get; set;
        }

        [CommandProperty]
        [DefaultValue((long)-1)]
        public long Revision
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
            var metaData = service.GetDataGenerationData(this.Address, DataBaseSettings.DataBaseName, DataBaseSettings.Tags, FilterSettings.FilterExpression, this.Devmode, this.Revision);

            this.Out.WriteLine("data serializing.");
            var serializer = this.serializers.FirstOrDefault(item => item.Name == this.OutputType);
            this.Serialize(serializer, metaData);
            this.Out.WriteLine("data serialized.");
        }

        private void Serialize(IDataSerializer serializer, SerializationSet metaData)
        {
            if (DataSplitSetting.Split)
            {
                this.SerializePerTable(serializer, metaData);
            }
            else
            {
                this.SerializeAll(serializer, metaData);
            }
        }

        private void SerializeAll(IDataSerializer serializer, SerializationSet metaData)
        {
            serializer.Serialize(this.Filename, metaData);
        }

        private void SerializePerTable(IDataSerializer serializer, SerializationSet metaData)
        {
            var metaDataList = new List<SerializationSet>();

            foreach (var table in metaData.Tables)
            {
                var filteredMetaData = metaData.Filter(table.Name);
                if (filteredMetaData.Tables.Any())
                {
                    metaDataList.Add(filteredMetaData);
                }
            }

            foreach (var dataSet in metaDataList)
            {
                var filepath = Path.Combine(this.Filename, $"{dataSet.Tables[0].Name}.{DataSplitSetting.Ext}");
                serializer.Serialize(filepath, dataSet);
            }
        }


        private TextWriter Out
        {
            get { return this.commandContext.Value.Out; }
        }
    }
}
