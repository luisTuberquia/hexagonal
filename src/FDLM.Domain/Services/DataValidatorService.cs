using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Domain.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Services
{
    internal class DataValidatorService : IDataValidator
    {
        private readonly IDomainResourceService _resource;
        private readonly ILogger<DataValidatorService> _logger;

        public DataValidatorService(IDomainResourceService resource, ILogger<DataValidatorService> logger) 
        {
            _resource = resource;
            _logger = logger;
        }

        public Results<bool> InputsAreNotNull(IList<Number> numbers)
        {
            Results<bool> results = new Results<bool>(true);

            try
            {
                if (numbers is null)
                {
                    results.AddError(_resource.GetString(DomainResource.NotAllowNullValues));
                    results.Result = false;
                    return results;
                }

                foreach (var number in numbers)
                {
                    if (number is null || number.Value is null)
                    {
                        results.AddError(_resource.GetString(DomainResource.NotAllowNullValues));
                        results.Result = false;
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Result = false;
                results.AddError(_resource.GetString(DomainResource.GenericValidationError));
                _logger.LogError(ex, _resource.GetString(DomainResource.GenericValidationError));
            }

            return results;
        }

        public Results<bool> InputsAreIntegerNumbers(IList<Number> numbers)
        {
            Results<bool> results = new Results<bool>(true);

            try
            {
                foreach (var number in numbers)
                {
                    if (!(number is IntegerNumber))
                    {
                        results.AddError(_resource.GetString(DomainResource.OnlyIntegerNumbers));
                        results.Result = false;
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Result = false;
                results.AddError(_resource.GetString(DomainResource.GenericValidationError));
                _logger.LogError(ex, _resource.GetString(DomainResource.GenericValidationError));
            }

            return results;
        }

        public Results<bool> InputsAreComplexNumbers(IList<Number> numbers)
        {
            Results<bool> results = new Results<bool>(true);
            try
            {
                foreach (var number in numbers)
                {
                    if (!(number is ComplexNumber))
                    {
                        results.AddError(_resource.GetString(DomainResource.OnlyComplexNumbers));
                        results.Result = false;
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                results.Result = false;
                results.AddError(_resource.GetString(DomainResource.GenericValidationError));
                _logger.LogError(ex, _resource.GetString(DomainResource.GenericValidationError));
            }

            return results;
        }

        public Results<bool> NumbersArePositive(IList<Number> numbers)
        {
            Results<bool> results = new Results<bool>(true);
            try
            {
                foreach (var number in numbers)
                {
                    if (number is IntegerNumber)
                    {
                        var integer = (IntegerNumber)number;
                        if (integer.IntValue < 0)
                        {
                            results.AddError(_resource.GetString(DomainResource.NumbersAreNegative));
                            results.Result = false;
                            return results;
                        }
                    }
                    if (number is ComplexNumber)
                    {
                        var complex = (ComplexNumber)number;
                        if (complex.RealValue < 0 || complex.ImaginaryValue < 0)
                        {
                            results.AddError(_resource.GetString(DomainResource.NumbersAreNegative));
                            results.Result = false;
                            return results;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                results.Result = false;
                results.AddError(_resource.GetString(DomainResource.GenericValidationError));
                _logger.LogError(ex, _resource.GetString(DomainResource.GenericValidationError));
            }

            return results;
        }        
    }
}
