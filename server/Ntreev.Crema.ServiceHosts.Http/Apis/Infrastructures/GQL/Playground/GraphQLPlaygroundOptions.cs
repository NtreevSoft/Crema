using System.Collections.Generic;
using Microsoft.Owin;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.GQL.Playground
{
    public class GraphQLPlaygroundOptions
    {
        public PathString Path { get; set; } = new PathString("/ui/playground");
        public PathString GraphQLEndPoint { get; set; } = new PathString("/api/v1/graphql");
        public Dictionary<string, object> GraphQLConfig { get; set; }
        public Dictionary<string, object> PlaygroundSettings { get; set; }
    }
}