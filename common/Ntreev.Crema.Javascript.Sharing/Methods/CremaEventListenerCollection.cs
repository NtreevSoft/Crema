using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    class CremaEventListenerCollection : List<CremaEventListener>
    {
        public new void Remove(CremaEventListener item)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this.GetHashCode(this[i]) == this.GetHashCode(item))
                {
                    this.RemoveAt(i);
                    break;
                }
            }
        }

        private int GetHashCode(CremaEventListener item)
        {
            if (item.Target is System.Runtime.CompilerServices.Closure c)
            {
                return c.Constants[0].GetHashCode();
            }
            throw new NotImplementedException();
        }
    }
}
