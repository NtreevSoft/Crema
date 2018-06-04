using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IObjectSerializer
    {
        string[] Serialize(object obj, string itemPath, PropertyCollection properties);

        object Deserialize(Type type, string itemPath, PropertyCollection properties);

        string[] VerifyPath(Type type, string itemPath, PropertyCollection properties);

        string[] GetItemPaths(string path, Type type, PropertyCollection properties);

        string Name { get; }
    }
}
