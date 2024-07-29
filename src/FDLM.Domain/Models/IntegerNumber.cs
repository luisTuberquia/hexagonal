using FDLM.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models
{
    public class IntegerNumber : Number
    {
        public int IntValue => (int)Value;

        public IntegerNumber(int value) : base(value)
        {
        }

        public override string ToString()
        {
            return $"{IntValue}";
        }
    }
}
