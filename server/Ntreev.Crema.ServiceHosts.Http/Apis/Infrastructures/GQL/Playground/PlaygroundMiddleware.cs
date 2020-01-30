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

using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.GQL.Playground
{
    [Export]
    public class PlaygroundMiddleware : OwinMiddleware
    {
        private readonly GraphQLPlaygroundOptions options;
        private PlaygroundPageModel pageModel;

        public PlaygroundMiddleware(OwinMiddleware next, GraphQLPlaygroundOptions options) : base(next)
        {
            this.options = options;
        }

        public override Task Invoke(IOwinContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = context.Request ?? throw new NullReferenceException(nameof(context.Request));

            return IsPlaygroundRequest(context.Request)
                ? InvokePlayground(context)
                : this.Next.Invoke(context);
        }

        private bool IsPlaygroundRequest(IOwinRequest request)
        {
            return request.Method.Equals("get", StringComparison.OrdinalIgnoreCase)
                   && request.Path.StartsWithSegments(this.options.Path);
        }

        private Task InvokePlayground(IOwinContext context)
        {
            var response = context.Response;
            response.ContentType = "text/html";
            response.StatusCode = 200;

            if (this.pageModel == null)
            {
                this.pageModel = new PlaygroundPageModel(this.options);
            }

            var data = Encoding.UTF8.GetBytes(this.pageModel.Render());
            return response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}