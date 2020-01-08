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

using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Ntreev.Crema.ServiceHosts.Http.Benchmark.Benchmarks
{
    [SimpleJob(targetCount:5)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class HttpBenchmark
    {
        public string Token { get; set; }
        private static string DatabaseName = "GameData_KOR_Dev";

        [GlobalSetup]
        public async Task<string> LoginAsync()
        {
            var client = HttpClientTest.GetHttpClient("http://localhost:4104");
            var response = await client.PostAsJsonAsync($"api/v1/commands/login/databases/{DatabaseName}/load-and-enter", new
            {
                UserID = "admin",
                Password = "admin"
            });
            var json = await response.Content.ReadAsDynamicAsync();
            var token = json.Token.ToString();
            this.Token = token;
            
            return token;
        }

        [GlobalCleanup]
        public async Task LogoutAsync()
        {
            var client = HttpClientTest.GetHttpClient("http://localhost:4104", token: this.Token);
            var response = await client.PostAsJsonAsync("api/v1/commands/logout", new
            {
                UserID = "admin",
                Password = "admin"
            });
            var json = await response.Content.ReadAsDynamicAsync();
        }

        [Benchmark(Description = "HttpApi")]
        public async Task<string> GetHttpApiAsync()
        {
            var client = HttpClientTest.GetHttpClient("http://localhost:4104", token: this.Token);
            var response = await client.GetAsync($"api/v1/commands/databases/{DatabaseName}/tables/*/info");
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        [Benchmark(Description = "GraphQL (All Fields)")]
        public async Task<string> GetGraphQLAllAsync()
        {
            var client = HttpClientTest.GetHttpClient("http://localhost:4104", token: this.Token);
            var response = await client.PostAsJsonAsync("api/v1/graphql", new
            {
                Query = @"
query {
  Database(name: ""GameData_KOR_Dev"") {
    ID
    Name
    TablesHashValue
    TypesHashValue
    Comment
    CreatedDateTime
    Creator
    ModifiedDateTime
    Modifier
    Paths
    Revision
    Tags
    Tables {
      ID
      Name
      TableName
      CategoryPath
      Comment
      Tags
      ContentsModifiedDateTime
      ContentsModifier
      CreatedDateTime
      Creator
      DerivedTags
      HashValue
      TemplatedParent
      Columns {
        ID
        Name
        IsKey
        DataType
        Tags
        AllowNull
        AutoIncrement
        CreatedDateTime
        Creator
        DefaultValue
        DerivedTags
        ReadOnly
        ModifiedDateTime
        Modifier
        IsUnique
      }
    }
  }
}
"
            });
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        [Benchmark(Description = "GraphQL (Specified)")]
        public async Task<string> GetGraphQLSpecifiedAsync()
        {
            var client = HttpClientTest.GetHttpClient("http://localhost:4104");
            client.DefaultRequestHeaders.Add("Token", this.Token);
            var response = await client.PostAsJsonAsync("api/v1/graphql", new
            {
                Query = @"
query {
  Database(name:""GameData_KOR_Dev"") {
  	ID
  	Name
  	IsEntered
  	Tags
  	Tables {
  		Name
  		Tags
  		Columns {
  			Name
  			DefaultValue
  			Tags
  		}
  	}
  }
}
"
            });
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
