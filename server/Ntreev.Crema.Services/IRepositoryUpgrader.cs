using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public interface IRepositoryUpgrader
    {
        string Upgrade(string sourcePath);

        IRepositoryProvider RepositoryProvider { get; }

        string Name { get; }
    }
}
