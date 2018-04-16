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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library;

namespace Ntreev.Crema.Bot
{
    [InheritedExport(typeof(IPlugin))]
    [Export]
    public class AutobotService : AutobotServiceBase, IPlugin
    {
        private readonly ICremaHost cremaHost;
        private Authentication authentication;

        [ImportingConstructor]
        public AutobotService(ICremaHost cremaHost, [ImportMany]IEnumerable<ITaskProvider> taskProviders)
            : base(cremaHost, taskProviders)
        {
            this.cremaHost = cremaHost;
        }

        public void Initialize(Authentication authentication)
        {
            this.authentication = authentication;
        }

        public void Release()
        {

        }

        protected override AutobotBase CreateInstance(string autobotID)
        {
            return new Autobot(this, this.cremaHost.Address, autobotID);
        }

        public string Name
        {
            get { return "bot"; }
        }

        public Guid ID => GuidUtility.FromName(this.Name);
    }
}