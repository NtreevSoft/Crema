using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ntreev.Crema.Data.Json
{
    public static class JsonSchemaUtility
    {
        private const string schemaKey = "$schema";

        private static readonly Dictionary<Type, JSchema> schemas = new Dictionary<Type, JSchema>();

        static JsonSchemaUtility()
        {

        }

        public static JSchema CreateSchema(Type type)
        {
            var generator = new JSchemaGenerator();
            var schema = generator.Generate(type);
            return schema;
        }

        public static JSchema CreateSchema(string[] enums)
        {
            var schema = CreateSchema(typeof(string));
            schema.SetEnums(enums);
            return schema;
        }

        public static string GetString(Type type)
        {
            var schema = CreateSchema(type);
            schema = JSchema.Parse(schema.ToString());
            schema.Properties.Add(schemaKey, JsonSchemaUtility.GetSchema(typeof(string)));
            return schema.ToString();
        }

        public static void SetEnums(this JSchema schema, string[] items)
        {
            schema.Enum.Clear();
            foreach (var item in items)
            {
                schema.Enum.Add(JValue.CreateString(item));
            }
        }

        public static void SetEnums(this JSchema schema, string propertyName, string[] items)
        {
            var prop = schema.Properties[propertyName];
            prop.SetEnums(items);
        }

        internal static JSchema GetSchema(Type type)
        {
            if (schemas.ContainsKey(type) == false)
            {
                var generator = new JSchemaGenerator();
                var schema = generator.Generate(type);
                WriteDescription(schema, type);
                schemas.Add(type, schema);
            }

            return schemas[type];
        }

        private static void WriteDescription(JSchema schema, Type type)
        {
            if (schema.Properties != null)
            {
                foreach (var item in schema.Properties)
                {
                    var prop = GetPropertyDescriptor(item.Key);
                    if (prop != null && prop.Description != string.Empty)
                        item.Value.Description = prop.Description;

                    WriteDescription(item.Value, prop.PropertyType);
                }
            }

            PropertyDescriptor GetPropertyDescriptor(string propertyName)
            {
                foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(type))
                {
                    var attr = item.Attributes[typeof(JsonPropertyAttribute)] as JsonPropertyAttribute;
                    if (attr == null)
                        continue;
                    var name = attr.PropertyName ?? item.Name;
                    if (name == propertyName)
                        return item;
                }
                return null;
            }
        }
    }
}
