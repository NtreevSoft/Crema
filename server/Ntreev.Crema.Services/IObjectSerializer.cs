using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IObjectSerializer
    {
        string[] Serialize(string itemPath, object obj, IDictionary properties);

        object Deserialize(string itemPath, Type type, IDictionary properties);

        string[] GetPath(string itemPath, Type type, IDictionary properties);

        string[] GetReferencedPath(string itemPath, Type type, IDictionary properties);

        string[] GetItemPaths(string path, Type type, IDictionary properties);

        string Name { get; }
    }
}
