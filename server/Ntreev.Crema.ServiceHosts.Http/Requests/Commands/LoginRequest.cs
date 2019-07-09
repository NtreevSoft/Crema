using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ntreev.Crema.ServiceHosts.Http.Requests.Commands
{
    public class LoginRequest
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
