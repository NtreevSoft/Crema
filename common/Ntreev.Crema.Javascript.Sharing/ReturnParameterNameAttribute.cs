using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Javascript
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ReturnParameterNameAttribute : Attribute
    {
        public ReturnParameterNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
        }
    }
}
