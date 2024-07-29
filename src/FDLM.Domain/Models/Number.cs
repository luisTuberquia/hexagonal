using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models
{
    public abstract class Number
    {        
        public object Value { get; private set; }

        public Number(object value) 
        { 
            Value = value;
        }
    }
}
