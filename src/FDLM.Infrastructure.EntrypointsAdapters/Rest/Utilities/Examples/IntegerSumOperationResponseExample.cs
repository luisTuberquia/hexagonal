using FDLM.Utilities;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using FDLM.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using FDLM.Domain.Models.Result;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples
{
    [ExcludeFromCodeCoverage]
    public class IntegerSumOperationResponseExample : IMultipleExamplesProvider<Results<CalculatorOperationResponse>>
    {
        public IEnumerable<SwaggerExample<Results<CalculatorOperationResponse>>> GetExamples()
        {
            ITools tools = new Tools();
            DateTime creationDate = DateTime.Now;

            CalculatorOperationResponse calcultorOperationResponse = new CalculatorOperationResponse
            {
                Id = $"{Operation.Sum}-{tools.ToUnixEpoch(creationDate)}",
                Operation = Operation.Sum,
                TypeNumbers = TypeNumber.Integer,
                Addends = new List<string> { "10", "20" },
                OperationResult = "30",
                CreationDate = creationDate.ToString("dd-MM-yyyy HH:mm:ss"),

            };

            Results<CalculatorOperationResponse> results = new Results<CalculatorOperationResponse>();
            results.Result = calcultorOperationResponse;

            yield return SwaggerExample.Create("IntegerSumOperationResponseExample", results);
        }
    }
}
