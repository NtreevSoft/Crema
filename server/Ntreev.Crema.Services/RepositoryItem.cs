using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public struct RepositoryItem
    {
        public string Path { get; set; }

        public string OldPath { get; set; }

        public RepositoryItemStatus Status { get; set; }
    }
}
