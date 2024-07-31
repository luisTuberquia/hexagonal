using FDLM.Application.Ports;
using FDLM.Application.Resources;
using FDLM.Domain.Constants;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Domain.Services;
using FDLM.Utilities;
using Microsoft.Extensions.Logging;

namespace FDLM.Application.UseCases.Calculator
{
    internal class CalculatorUseCase : ICalculatorUseCase
    {
        private readonly IDataValidator _dataValidator;
        private readonly ICalculatorOperationRepositoryPort _operationsRepositoryPort;
        private readonly ITools _tools;
        private readonly Dictionary<TypeNumber, IOperation> _calculators;
        private readonly IApplicationResourceService _resources;
        private readonly ILogger<CalculatorUseCase> _logger;

        public CalculatorUseCase(Func<string, IOperation> serviceAccessor,
                                IDataValidator dataValidator,
                                ICalculatorOperationRepositoryPort operationsRepositoryPort,
                                ITools tools,
                                IApplicationResourceService resources,
                                ILogger<CalculatorUseCase> logger)
        {
            _calculators = new Dictionary<TypeNumber, IOperation>
            {
                { TypeNumber.Integer, serviceAccessor(Constants.Services.IntegerOperationService) },
                { TypeNumber.Complex, serviceAccessor(Constants.Services.ComplexOperationService) }
            };

            _dataValidator = dataValidator;
            _operationsRepositoryPort = operationsRepositoryPort;
            _tools = tools;
            _resources = resources;
            _logger = logger;
        }

        #region Public Methods
        public async Task<Results<Number>> SumAsync(IList<Number> addends)
        {
            var results = new Results<Number>();

            try
            {
                TypeNumber typeNumber = GetTypeNumber(addends);
                IOperation calculator = null;
                if (_calculators.TryGetValue(typeNumber, out calculator))
                {
                    var sumResult = await calculator.SumAsync(addends);
                    results.Result = sumResult.Result;
                    results.TotalItemsReturned = sumResult.TotalItemsReturned;
                    results.TotalItemsInDataBase = sumResult.TotalItemsInDataBase;
                    results.AddErrors(sumResult.Errors);
                    return results;
                }
                else
                {
                    results.AddError(_resources.GetString(ApplicationResource.OnlyNumbers));
                }
            }
            catch (Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        public async Task<Results<CalculatorOperation>> SaveOperationAsync(CalculatorOperation calculatorOperation)
        {
            var results = new Results<CalculatorOperation>();

            try
            {
                calculatorOperation.Id = GenerateId(calculatorOperation);
                calculatorOperation.CreationDateUtc = DateTime.UtcNow;

                var saveResult = await _operationsRepositoryPort.SaveAsync(calculatorOperation);
                if (saveResult.HasErrors)
                {
                    results.AddErrors(saveResult.Errors);
                }

                results.Result = saveResult.Result;
                results.TotalItemsReturned = saveResult.TotalItemsReturned;
                results.TotalItemsInDataBase = saveResult.TotalItemsInDataBase;
            }
            catch (Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        public async Task<Results<CalculatorOperation>> SumAndSaveOperationAsync(IList<Number> addends)
        {
            var results = new Results<CalculatorOperation>();
            try
            {
                var sumResult = await SumAsync(addends);
                if (sumResult.IsSuccess)
                {
                    var calculatorOperation = new CalculatorOperation()
                    {
                        Operation = Operation.Sum,
                        Addends = addends.Select(a => a != null ? a.ToString() : null).ToList(),
                        OperationResult = sumResult.Result.ToString(),
                        TypeNumbers = GetTypeNumber(addends),
                        CreationDateUtc = DateTime.UtcNow
                    };

                    var saveResult = await SaveOperationAsync(calculatorOperation);
                    if (saveResult.HasErrors)
                    {
                        results.AddErrors(saveResult.Errors);
                    }

                    results.Result = calculatorOperation;
                    results.TotalItemsReturned = saveResult.TotalItemsReturned;
                    results.TotalItemsInDataBase = saveResult.TotalItemsInDataBase;
                }
                else
                {
                    results.AddErrors(sumResult.Errors);
                }
            }
            catch (Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        public async Task<Results<IList<CalculatorOperation>>> GetOperationsByCreationDateAsync(DateTime startDateUtc, DateTime endDateUtc, int pageNumber, int pageSize, string sortField, SortDirection sortDirection)
        {
            var results = new Results<IList<CalculatorOperation>>();
            try
            {
                var findResult = await _operationsRepositoryPort.FindByCreationDateAsync(startDateUtc, endDateUtc, pageNumber, pageSize, sortField, sortDirection);
                if (findResult.IsSuccess)
                {
                    results.Result = findResult.Result;
                    results.TotalItemsInDataBase = findResult.TotalItemsInDataBase;
                    results.TotalItemsReturned = findResult.TotalItemsReturned;
                }
                else
                {
                    results.AddErrors(findResult.Errors);
                }
            }
            catch (Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        public async Task<Results<CalculatorOperation>> GetOperationAsync(string id)
        {
            var results = new Results<CalculatorOperation>();
            try
            {
                var findResult = await _operationsRepositoryPort.FindByIdAsync(id);
                if (findResult.IsSuccess)
                {
                    results.Result = findResult.Result;
                    results.TotalItemsInDataBase = findResult.TotalItemsInDataBase;
                    results.TotalItemsReturned = findResult.TotalItemsReturned;
                }
                else
                {
                    results.AddErrors(findResult.Errors);
                }
            }
            catch (Exception ex)
            {
                results.AddError(ex);
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }
        #endregion

        #region Private Methods
        private string GenerateId(CalculatorOperation calculatorOperation)
        {
            return $"{calculatorOperation.Operation}-{_tools.ToUnixEpoch(calculatorOperation.CreationDateUtc)}";
        }

        private TypeNumber GetTypeNumber(IList<Number> numbers)
        {
            TypeNumber typeNumber = TypeNumber.Unknow;

            int integerCount = 0;
            int complexCount = 0;
            int decimalCount = 0;
            foreach (var number in numbers)
            {
                if (number is ComplexNumber)
                {
                    complexCount++;
                }
                else if (number is IntegerNumber)
                {
                    integerCount++;
                }
                else if (number is DecimalNumber)
                {
                    decimalCount++;
                }
                else
                {
                    return TypeNumber.Unknow;
                }
            }

            if (integerCount > 0 && complexCount > 0 || decimalCount > 0 && complexCount > 0)
            {
                return TypeNumber.Unknow;
            }

            if (integerCount > 0)
            {
                return TypeNumber.Integer;
            }
            if (complexCount > 0)
            {
                return TypeNumber.Complex;
            }

            if (decimalCount > 0)
            {
                return TypeNumber.Decimal;
            }

            return typeNumber;
        }
        #endregion
    }
}
