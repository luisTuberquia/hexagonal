using Microsoft.Extensions.Logging;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Services
{
    internal class IntegerOperationService : IOperation
    {
        private readonly IDataValidator _dataValidator;
        private readonly ILogger<IntegerOperationService> _logger;

        public IntegerOperationService(IDataValidator dataValidator, ILogger<IntegerOperationService> logger) 
        { 
            _dataValidator = dataValidator;
            _logger = logger;
        }

        public async Task<Results<Number>> SumAsync(IList<Number> addends)
        {
            var results = new Results<Number>();
            try
            {
                var validationsResult = InputsAreValid(addends);
                if (validationsResult.HasErrors)
                {
                    results.AddErrors(validationsResult.Errors);
                    return results;
                }

                var numbers = Map(addends);
                int sum = 0;
                foreach (IntegerNumber i in numbers)
                {
                    sum += i.IntValue;
                }

                results.Result = new IntegerNumber(sum);
            }
            catch(Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);                
            }

            return results;
        }

        private Results<bool> InputsAreValid(IList<Number> addends)
        {
            var results = new Results<bool>(true);

            var validationResult = _dataValidator.InputsAreNotNull(addends);
            if (validationResult.HasErrors)
            {
                results.AddErrors(validationResult.Errors);
                return results;
            }

            validationResult = _dataValidator.InputsAreIntegerNumbers(addends);
            if (validationResult.HasErrors)
            {
                results.AddErrors(validationResult.Errors);
                return results;
            }

            validationResult = _dataValidator.NumbersArePositive(addends);
            if (validationResult.HasErrors)
            {
                results.AddErrors(validationResult.Errors);
                return results;
            }

            return results;
        }

        private IList<IntegerNumber> Map(IList<Number> addends)
        {
            var numbers = new List<IntegerNumber>();
            foreach (var addend in addends)
            {
                numbers.Add((IntegerNumber)addend);
            }

            return numbers;
        }
    }
}
