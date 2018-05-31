using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IObjectSerializer
    {
        string[] Serialize(object obj, string itemPath, object state);

        object Deserialize(Type type, string itemPath);

        string[] VerifyPath(Type type, string itemPath, object state);

        string Name { get; }
    }
}
