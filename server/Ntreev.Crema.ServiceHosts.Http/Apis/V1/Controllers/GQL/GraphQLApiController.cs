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

using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using GraphQL;
using GraphQL.Conversion;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.GQL;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.GQL
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1")]
    public class GraphQLApiController : CremaApiController
    {
        private readonly ISchema schema;
        private readonly IDocumentExecuter executer;
        private readonly IDocumentWriter writer;

        [ImportingConstructor]
        public GraphQLApiController(ICremaHost cremaHost,
            ISchema schema,
            IDocumentExecuter executer,
            IDocumentWriter writer)
            : base(cremaHost)
        {
            this.schema = schema;
            this.executer = executer;
            this.writer = writer;
        }

        [HttpPost]
        [Route("graphql")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostGraphQLAsync(GraphQLQueryRequest request)
        {
            var inputs = request.Variables.ToInputs();
            var queryToExecute = request.Query;

            var result = await this.executer.ExecuteAsync(_ =>
            {
                _.Schema = this.schema;
                _.Query = queryToExecute;
                _.OperationName = request.OperationName;
                _.Inputs = inputs;
                _.UserContext = this.ContainsCremaToken ? this.Authentication : null;

                _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                _.ExposeExceptions = true;
                _.FieldNameConverter = new DefaultFieldNameConverter();
            }).ConfigureAwait(false);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            var json = await this.writer.WriteToStringAsync(result);
            var response = this.Request.CreateResponse(httpResult);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }
    }
}
