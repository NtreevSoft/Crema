using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IObjectSerializerEvent
    {
        string[] Create(IObjectSerializer serializer, string itemPath, object data);

        object Rename(IObjectSerializer serializer, object obj, string itemPath, string newItemPath);

        void Move(object obj, string itemPath, string newItemPath);

        void Delete(object obj);

        bool CanSupport(Type type);

        string Name { get; }
    }
}
