using FDLM.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models
{
    public class ComplexNumber : Number
    {
        public int ImaginaryValue { get; private set; }
        public int RealValue => (int)Value;


        public ComplexNumber(int realValue, int imaginaryValue) : base(realValue)
        {
            ImaginaryValue = imaginaryValue;
        }

        public override string ToString()
        {
            return $"{RealValue}i{ImaginaryValue}";
        }
    }
}
