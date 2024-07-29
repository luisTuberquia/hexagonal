using Microsoft.Extensions.Logging;
using Serilog;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Services
{
    internal class ComplexOperationService : IOperation
    {
        private readonly IDataValidator _dataValidator;
        private readonly ILogger<ComplexOperationService> _logger;

        public ComplexOperationService(IDataValidator dataValidator, ILogger<ComplexOperationService> logger) 
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

                var numbers = await Map(addends);

                int realSum = 0;
                int imaginarySum = 0;
                foreach (ComplexNumber i in numbers)
                {
                    realSum += i.RealValue;
                    imaginarySum += i.ImaginaryValue;
                }

                results.Result = new ComplexNumber(realSum, imaginarySum);
            }
            catch (Exception ex)
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

            validationResult = _dataValidator.InputsAreComplexNumbers(addends);
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

        private Task<IList<ComplexNumber>> Map(IList<Number> addends)
        {
            IList<ComplexNumber> numbers = new List<ComplexNumber>();
            foreach (var addend in addends)
            {
                numbers.Add((ComplexNumber)addend);
            }

            return Task.FromResult(numbers);
        }

        
    }
}
