using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Swagger
{
    class DefaultValueSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            if (schema.properties == null || !schema.properties.Any()) return;

            foreach(var property in schema.properties)
            {
                var propertyInfo = type.GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var defaultValueAttribute = propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() as DefaultValueAttribute;
                if (defaultValueAttribute == null) continue;

                property.Value.@default = defaultValueAttribute.Value;
            }
        }
    }
}
