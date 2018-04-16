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
using Ntreev.Crema.Commands;
using Ntreev.Crema.Commands.Consoles;
using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Javascript
{
    [Export]
    public class ScriptContext : ScriptContextBase
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public ScriptContext(ICremaHost cremaHost)
            : base("Crema Client", cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        protected override IScriptMethodContext CreateContext(object state)
        {
            if (state is ScriptMethodContext context)
                return context;
            return new ScriptMethodContext(this, this.cremaHost, null);
        }

        protected override IScriptMethod[] CreateMethods()
        {
            var items = this.cremaHost.GetService(typeof(IEnumerable<IScriptMethod>)) as IEnumerable<IScriptMethod>;

            foreach (var item in items)
            {
                var attr = Attribute.GetCustomAttribute(item.GetType(), typeof(PartCreationPolicyAttribute)) as PartCreationPolicyAttribute;
                if (attr == null)
                    throw new NotSupportedException();
                if (attr.CreationPolicy != CreationPolicy.NonShared)
                    throw new NotSupportedException();
            }

            return items.ToArray();
        }

        internal void RunInternal(string scripts, Authentication authentication, IDictionary<string, object> properties)
        {
            var context = new ScriptMethodContext(this, this.cremaHost, authentication);
            this.Run(scripts, string.Empty, properties, context);
        }

        internal async void RunAsyncInternal(string scripts, Authentication authentication, IDictionary<string, object> properties)
        {
            try
            {
                var context = new ScriptMethodContext(this, this.cremaHost, authentication);
                await this.RunAsync(scripts, string.Empty, properties, context);
            }
            catch (Exception e)
            {
                this.Out.WriteLine(e.Message);
            }
        }
    }
}
