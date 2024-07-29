using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Services
{
    public interface IDataValidator
    {
        Results<bool> InputsAreNotNull(IList<Number> numbers);
        Results<bool> InputsAreIntegerNumbers(IList<Number> numbers);
        Results<bool> InputsAreComplexNumbers(IList<Number> numbers);
        Results<bool> NumbersArePositive(IList<Number> numbers);
    }
}
