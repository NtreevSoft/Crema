using System;
using BenchmarkDotNet.Attributes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceHosts.Http.Benchmark.Benchmarks
{
    [SimpleJob(targetCount: 5)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class HttpBenchmark
	{
		private static readonly string DatabaseName;
        private const string address = "http://localhost:4104";
        private const string userId = "admin";
        private const string password = "admin";
        private string token;

        static HttpBenchmark()
		{
			DatabaseName = "GameData_KOR_Dev";
		}

        [Benchmark(Description="GraphQL (All Fields)")]
		public async Task<string> GetGraphQLAllFieldsAsync()
        {
            _ = this.token ?? throw new InvalidOperationException(nameof(this.token));

			var httpClient = HttpClientTest.GetHttpClient(address, null, this.token);
			var httpResponseMessage = await httpClient.PostAsJsonAsync("api/v1/graphql", 
                new
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
}"
                });
			return await httpResponseMessage.Content.ReadAsStringAsync();
		}

		[Benchmark(Description="GraphQL (Specified)")]
		public async Task<string> GetGraphQLSpecifiedFieldsAsync()
		{
            _ = this.token ?? throw new InvalidOperationException(nameof(this.token));

            var httpClient = HttpClientTest.GetHttpClient(address);
			httpClient.DefaultRequestHeaders.Add("Token", this.token);
            var httpResponseMessage = await httpClient.PostAsJsonAsync("api/v1/graphql",
                new
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
}"
                });
			return await httpResponseMessage.Content.ReadAsStringAsync();
		}

		[Benchmark(Description="HttpApi")]
		public async Task<string> GetHttpApiAsync()
		{
            _ = this.token ?? throw new InvalidOperationException(nameof(this.token));

            var httpClient = HttpClientTest.GetHttpClient(address, null, this.token);
			var httpResponseMessage = await httpClient.GetAsync($"api/v1/commands/databases/{HttpBenchmark.DatabaseName}/tables/*/info");
			return await httpResponseMessage.Content.ReadAsStringAsync();
		}

		[GlobalSetup]
		public async Task<string> LoginAsync()
		{
			var httpClient = HttpClientTest.GetHttpClient(address);
			var httpResponseMessage = await httpClient.PostAsJsonAsync($"api/v1/commands/login/databases/{HttpBenchmark.DatabaseName}/load-and-enter", new { UserID = userId, Password = password });
			var response = await JsonExtensions.ReadAsDynamicAsync(httpResponseMessage.Content);
            this.token = response.Token.ToString();
			return response.Token.ToString();
		}

		[GlobalCleanup]
		public async Task LogoutAsync()
		{
            _ = this.token ?? throw new InvalidOperationException(nameof(this.token));

            var httpClient = HttpClientTest.GetHttpClient(address, null, this.token);
			var httpResponseMessage = await httpClient.PostAsJsonAsync("api/v1/commands/logout", new { UserID = userId, Password = password });
			await JsonExtensions.ReadAsDynamicAsync(httpResponseMessage.Content);
		}
	}
}