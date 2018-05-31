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
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands
{
    [Export(typeof(ICommand))]
    [Summary("사용할 수 있는 저장소 모듈의 목록을 표시합니다.")]
    [Description("x86 혹은 x64 플랫폼 대상에 따라 여러 저장소 모듈이 지원됩니다. 저장소 모듈은 크레마 실행파일 위치내에 repo-modules폴더의 하위 폴더로 존재하며 별도의 설정 없을때는 자동으로 모듈을 선택합니다.")]
    class RepositoryModulesCommands : CommandBase
    {
        private readonly IRepositoryProvider[] repoProviders;

        [ImportingConstructor]
        public RepositoryModulesCommands([ImportMany]IEnumerable<IRepositoryProvider> repoProviders)
            : base("repo-modules")
        {
            this.repoProviders = repoProviders.ToArray();
        }

        protected override void OnExecute()
        {
            Console.WriteLine("repository modules");

            //var defaultModule = CremaBootstrapper.DefaultRepositoryModule;

            foreach (var item in this.repoProviders)
            {
                Console.Write("    ");

                //if (item.Name == defaultModule)
                //    Console.WriteLine("* {0}", item.Name);
                //else
                    Console.WriteLine("  {0}", item.Name);
            }
        }
    }
}
