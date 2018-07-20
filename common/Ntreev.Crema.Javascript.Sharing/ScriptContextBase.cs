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

using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ntreev.Crema.Commands;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Javascript
{
    public abstract class ScriptContextBase
    {
        private readonly string name;
        private readonly ICremaHost cremaHost;
        private readonly List<Engine> engineList = new List<Engine>();
        private Authentication authentication;
        private TextWriter writer;

        public ScriptContextBase(string name, ICremaHost cremaHost)
        {
            this.name = name;
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += CremaHost_Opened;
            this.cremaHost.Closed += CremaHost_Closed;
        }

        public static Dictionary<string, Type> GetArgumentTypes(string[] arguments)
        {
            var properties = new Dictionary<string, Type>(arguments.Length);
            foreach (var item in arguments)
            {
                if (CommandStringUtility.TryGetKeyValue(item, out var key, out var value) == true)
                {
                    var typeName = value;
                    if (CommandStringUtility.IsWrappedOfQuote(value))
                    {
                        value = CommandStringUtility.TrimQuot(value);
                    }

                    if (value == "number")
                    {
                        properties.Add(key, typeof(decimal));
                    }
                    else if (value == "boolean")
                    {
                        properties.Add(key, typeof(bool));
                    }
                    else if (value == "string")
                    {
                        properties.Add(key, typeof(string));
                    }
                    else
                    {
                        throw new ArgumentException(typeName);
                    }
                }
                else
                {
                    throw new ArgumentException(item);
                }
            }
            return properties;
        }

        public void Run(string script, string functionName, IDictionary<string, object> properties, object state)
        {
            if (functionName == null)
                throw new ArgumentNullException(nameof(functionName));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var context = this.CreateContext(state);
            var engine = new Engine(cfg => cfg.CatchClrExceptions());
            foreach (var item in properties)
            {
                engine.SetValue(item.Key, item.Value);
                context.Properties.Add(item.Key, item.Value);
            }

            var methodItems = this.CreateMethods();
            foreach (var item in methodItems)
            {
                item.Context = context;
                context.Properties[item.GetType()] = item;
                engine.SetValue(item.Name, item.Delegate);
            }
            foreach (var item in methodItems)
            {
                if (item is ScriptMethodBase methodBase)
                {
                    methodBase.Initialize();
                }
            }

            var enumTypes = this.GetEnums(methodItems);
            foreach (var item in enumTypes)
            {
                var names = Enum.GetNames(item);
                var dictionary = new Dictionary<string, decimal>(names.Length);
                foreach (var name in names)
                {
                    dictionary.Add(name, Convert.ToDecimal(Enum.Parse(item, name)));
                }
                engine.SetValue(item.Name, dictionary);
            }

            try
            {
                lock (this.engineList)
                {
                    this.engineList.Add(engine);
                }
                engine.Execute(script);
                if (functionName != string.Empty)
                {
                    engine.Invoke(functionName);
                }
            }
            finally
            {
                lock (this.engineList)
                {
                    this.engineList.Remove(engine);
                }
                foreach (var item in methodItems)
                {
                    if (item is ScriptMethodBase methodBase)
                    {
                        methodBase.Dispose();
                    }
                }
            }
        }

        public void RunFromFile(string filename, string functionName, IDictionary<string, object> properties, object state)
        {
            this.Run(File.ReadAllText(filename), functionName, properties, state);
        }

        public Task RunAsync(string script, string functionName, IDictionary<string, object> properties, object state)
        {
            return Task.Run(() => this.Run(script, functionName, properties, state));
        }

        public string GenerateDeclaration()
        {
            return this.GenerateDeclaration(new Dictionary<string, Type>());
        }

        public string GenerateDeclaration(IDictionary<string, Type> properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// declaration for {this.name}");
            sb.AppendLine();

            var methodItems = this.CreateMethods().OrderBy(item => item.Name);
            foreach (var item in methodItems)
            {
                if (item is ScriptMethodBase methodBase)
                {
                    methodBase.Initialize();
                }
            }
            try
            {
                this.WriteProperties(sb, properties);
                this.WriteEnums(sb, methodItems);
                this.WriteDelegates(sb, methodItems);
                this.WriteMethods(sb, methodItems);

                return sb.ToString();
            }
            finally
            {
                foreach (var item in methodItems)
                {
                    if (item is ScriptMethodBase methodBase)
                    {
                        methodBase.Dispose();
                    }
                }
            }
        }

        public string GenerateArguments(IDictionary<string, Type> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var sb = new StringBuilder();
            sb.AppendLine($"// arguments for {this.name}");

            if (properties.Any() == true)
                sb.AppendLine();

            foreach (var item in properties)
            {
                sb.AppendLine($"declare var {item.Key}: {this.GetTypeString(item.Value)}; ");
            }

            return sb.ToString();
        }

        public string GetString(object value)
        {
            if (value != null && ScriptContextBase.IsDictionaryType(value.GetType()))
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            else if (value is System.Dynamic.ExpandoObject exobj)
            {
                return JsonConvert.SerializeObject(exobj, Formatting.Indented, new ExpandoObjectConverter());
            }
            else if (value != null && value.GetType().IsArray)
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            else if (value is bool b)
            {
                return b.ToString().ToLower();
            }
            else
            {
                return value.ToString();
            }
        }

        public TextWriter Out
        {
            get { return this.writer ?? Console.Out; }
            set
            {
                this.writer = value;
            }
        }

        protected abstract IScriptMethod[] CreateMethods();

        protected abstract IScriptMethodContext CreateContext(object state);

        protected void Initialize(Authentication authentication)
        {
            this.authentication = authentication;
        }

        protected void Release()
        {
#if SERVER
            if (this.authentication != null)
            {
                this.authentication.Expired -= Authentication_Expired;
            }
#endif
            this.authentication = null;
        }

        protected Authentication Authentication
        {
            get { return this.authentication; }
        }

#if SERVER
        private void Authentication_Expired(object sender, EventArgs e)
        {

        }
#endif

        private void CremaHost_Opened(object sender, EventArgs e)
        {

        }

        private void CremaHost_Closed(object sender, ClosedEventArgs e)
        {
            var items = this.engineList.ToArray();
            foreach (var item in items)
            {
                item.LeaveExecutionContext();
            }
        }

        private string GetParameterString(ParameterInfo p)
        {
            if (IsNullableType(p.ParameterType) == true)
            {
                return $"{p.Name}?: {this.GetTypeString(p.ParameterType.GetGenericArguments().First())}";
            }
            else
            {
                return $"{p.Name}: {this.GetTypeString(p.ParameterType)}";
            }
        }

        private string GetNameTypeString(string name, Type type)
        {
            if (IsNullableType(type) == true)
            {
                return $"{name}?: {this.GetTypeString(type.GetGenericArguments().First())}";
            }
            else
            {
                return $"{name}: {this.GetTypeString(type)}";
            }
        }

        private string GetTypeString(Type type)
        {
            if (type.IsArray == true)
                return this.GetTypeString(type.GetElementType()) + "[]";
            else if (type.IsEnum == true)
                return type.Name;
            else if (type == typeof(object))
                return "any";
            else if (type == typeof(bool))
                return "boolean";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(void))
                return "void";
            else if (type == typeof(int))
                return "number";
            else if (type == typeof(uint))
                return "number";
            else if (type == typeof(long))
                return "number";
            else if (type == typeof(ulong))
                return "number";
            else if (type == typeof(float))
                return "number";
            else if (type == typeof(double))
                return "number";
            else if (type == typeof(decimal))
                return "number";
            else if (IsDelegateType(type))
            {
                return type.Name;
                //var methodInfo = type.GetMethod("Invoke");
                //var parameters = methodInfo.GetParameters();
                //var argList = new List<string>();
                //for (var i = 0; i < parameters.Length; i++)
                //{
                //    argList.Add(this.GetNameTypeString(parameters[i].Name, parameters[i].ParameterType));
                //}
                //return $"type {type.Name} = ({string.Join(", ", argList)}) => {this.GetTypeString(methodInfo.ReturnType)}";
            }
            else if (IsActionType(type))
            {
                var sb = new StringBuilder();
                var argList = new List<string>();
                for (var i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    argList.Add(this.GetNameTypeString($"e{i + 1}", type.GetGenericArguments()[i]));
                }

                return $"({string.Join(", ", argList)}) => {this.GetTypeString(typeof(void))}";
            }
            else if (IsDictionaryType(type))
            {
                var keyType = type.GetGenericArguments()[0];
                var valueType = type.GetGenericArguments()[1];
                return $"{{ [key: {this.GetTypeString(keyType)}]: {this.GetTypeString(valueType)}; }}";
            }
            return "number";
        }

        private void WriteProperties(StringBuilder sb, IDictionary<string, Type> properties)
        {
            foreach (var item in properties)
            {
                sb.AppendLine($"declare var {item.Key}: {this.GetTypeString(item.Value)}; ");
            }

            if (properties.Any() == true)
                sb.AppendLine();
        }

        private void WriteEnums(StringBuilder sb, IEnumerable<IScriptMethod> methodItems)
        {
            var enumTypes = this.GetEnums(methodItems).ToArray();

            foreach (var item in enumTypes)
            {
                sb.AppendLine($"declare enum {item.Name} {{");

                var index = 0;
                foreach (var name in Enum.GetNames(item))
                {
                    if (index > 0)
                        sb.AppendLine(",");
                    sb.Append($"    {name} = {Convert.ToDecimal(Enum.Parse(item, name))}");
                    index++;
                }
                sb.AppendLine();
                sb.AppendLine("}");
            }

            if (enumTypes.Any())
                sb.AppendLine();
        }

        private void WriteDelegates(StringBuilder sb, IEnumerable<IScriptMethod> methodItems)
        {
            var delegateTypes = this.GetDelegates(methodItems).ToArray();

            foreach (var item in delegateTypes)
            {
                var methodInfo = item.GetMethod("Invoke");
                var parameters = methodInfo.GetParameters();
                var argList = new List<string>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    argList.Add(this.GetNameTypeString(parameters[i].Name, parameters[i].ParameterType));
                }
                sb.AppendLine($"type {item.Name} = ({string.Join(", ", argList)}) => {this.GetTypeString(methodInfo.ReturnType)};");
            }

            if (delegateTypes.Any())
                sb.AppendLine();
        }

        private void WriteMethods(StringBuilder sb, IEnumerable<IScriptMethod> methodItems)
        {
            var query = from item in methodItems
                        group item by this.GetCategory(item) into groupItem
                        orderby groupItem.Key
                        select groupItem;

            var comparer = new Comparer();
            foreach (var groupItem in query.OrderBy(i => i.Key, comparer))
            {
                sb.AppendLine($"// {groupItem.Key}");
                foreach (var item in groupItem)
                {
                    var methodInfo = item.Delegate.Method;
                    var parameters = methodInfo.GetParameters();

                    if (methodInfo.GetCustomAttribute<ReturnParameterNameAttribute>() is ReturnParameterNameAttribute attr)
                    {
                        if (methodInfo.ReturnType == typeof(void))
                            throw new InvalidOperationException($"{item.Name} does not have return parameter.");
                        sb.AppendLine($"/** @returns {attr.Name} */");
                    }

                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        if (parameter.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute descAttr)
                        {
                            sb.AppendLine($"/** @param {parameter.Name} {descAttr.Description} */");
                        }
                    }

                    sb.Append($"declare function {item.Name}");
                    sb.Append("(");

                    var argsString = string.Join(", ", methodInfo.GetParameters().Select(i => this.GetParameterString(i)));
                    sb.Append(argsString);
                    sb.Append($"): {this.GetTypeString(methodInfo.ReturnType)};");

                    sb.AppendLine();
                }
                sb.AppendLine();
            }
        }

        private string GetCategory(object item)
        {
            if (Attribute.GetCustomAttribute(item.GetType(), typeof(CategoryAttribute), false) is CategoryAttribute categoryAttr)
            {
                if (string.IsNullOrEmpty(categoryAttr.Category) == true)
                    return CategoryAttribute.Default.Category;
                return categoryAttr.Category;
            }
            return CategoryAttribute.Default.Category;
        }

        private IEnumerable<Type> GetEnums(IEnumerable<IScriptMethod> methodItems)
        {
            var query = from methodItem in methodItems
                        let methodInfo = methodItem.Delegate.Method
                        from parameterInfo in methodInfo.GetParameters()
                        where parameterInfo.ParameterType.IsEnum
                        select parameterInfo.ParameterType;
            return query.Distinct();
        }

        private IEnumerable<Type> GetDelegates(IEnumerable<IScriptMethod> methodItems)
        {
            var query = from methodItem in methodItems
                        let methodInfo = methodItem.Delegate.Method
                        from parameterInfo in methodInfo.GetParameters()
                        where IsDelegateType(parameterInfo.ParameterType)
                        select parameterInfo.ParameterType;
            return query.Distinct();
        }

        internal static bool IsDictionaryType(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) || type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        internal static bool IsActionType(Type type)
        {
            return type.Name.StartsWith("Action`");
        }

        internal static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static bool IsDelegateType(Type type)
        {
            if (IsActionType(type) == true)
                return false;
            if (type.BaseType != typeof(MulticastDelegate))
                return false;
            var methodInfo = type.GetMethod("Invoke");
            if (methodInfo == null)
                return false;
            return true;
        }

        #region classes

        class Comparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var x1 = x == CategoryAttribute.Default.Category ? string.Empty : x;
                var y1 = y == CategoryAttribute.Default.Category ? string.Empty : y;
                return x1.CompareTo(y1);
            }
        }

        #endregion
    }
}
