using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Resources
{
    public class DomainResource
    {
        public readonly static string NotAllowNullValues = "NotAllowNullValues";
        public readonly static string OnlyComplexNumbers = "OnlyComplexNumbers";
        public readonly static string OnlyIntegerNumbers = "OnlyIntegerNumbers";
        public readonly static string NumbersAreNegative = "NumbersAreNegative";
        public readonly static string GenericValidationError = "GenericValidationError";        
    }
}
