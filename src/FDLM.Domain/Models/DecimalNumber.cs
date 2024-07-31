using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models
{
    public class DecimalNumber : Number
    {

        public decimal DecimalValue => (decimal) Value;

        public DecimalNumber(decimal value) : base(value)
        {
        }

        public override string ToString()
        {
            return $"{DecimalValue}";
        }
    }
}
