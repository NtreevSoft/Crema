using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableTemplate))]
    class AddTableTemplateColumnMethod : DomainScriptMethodBase
    {
        private const string typeNameDescription = "boolean, string, int, float, double, dateTime, unsignedInt, long, short, unsignedLong, unsignedByte, duration, unsignedShort, byte, guid or typePath(e.g., /categoryPath/typeName)";

        [ImportingConstructor]
        public AddTableTemplateColumnMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Action<string, string, string, string, bool?>(this.AddTableTemplateColumn);
        }

        private void AddTableTemplateColumn(string domainID, string columnName,
            [Description(typeNameDescription)]
            string typeName, string comment, bool? isKey)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            var template = this.GetDomainHost<ITableTemplate>(domainID);
            var authentication = this.Context.GetAuthentication(this);
            template.Dispatcher.Invoke(() =>
            {
                var column = template.AddNew(authentication);
                column.SetName(authentication, columnName);
                column.SetDataType(authentication, typeName);
                if (comment != null)
                    column.SetComment(authentication, comment);
                if (isKey != null)
                    column.SetIsKey(authentication, isKey.Value);
                template.EndNew(authentication, column);
            });
        }
    }
}
