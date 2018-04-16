using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot
{
    public struct TaskStackItem
    {
        public object Target { get; set; }

        public int Count { get; set; }

        public override string ToString()
        {
            return this.Target?.ToString();
        }
    }
}
