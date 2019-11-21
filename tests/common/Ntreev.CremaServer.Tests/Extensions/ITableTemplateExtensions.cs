using Ntreev.Crema.Services;

namespace Ntreev.CremaServer.Tests.Extensions
{
    public static class ITableTemplateExtensions
    {
        public static ITableTemplate CreateAddNew(this ITableTemplate template, Authentication authentication, string columnName,
            string defaultValue = null, bool isKey = false, string dataType = null, bool? allowNull = null)
        {
            var column = template.AddNew(authentication);
            column.SetName(authentication, columnName);
            column.SetIsKey(authentication, isKey);
            if (dataType != null)
            {
                column.SetDataType(authentication, dataType);
            }

            if (allowNull != null)
            {
                column.SetAllowNull(authentication, allowNull.Value);
            }

            if (defaultValue != null)
            {
                column.SetDefaultValue(authentication, defaultValue);
            }

            template.EndNew(authentication, column);
            return template;
        }
    }
}
