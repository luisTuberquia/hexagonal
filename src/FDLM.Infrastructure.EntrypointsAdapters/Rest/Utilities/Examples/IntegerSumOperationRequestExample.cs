using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using Microsoft.AspNetCore.Http;
using FDLM.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples
{
    [ExcludeFromCodeCoverage]
    public class IntegerSumOperationRequestExample : IMultipleExamplesProvider<CalculatorOperationRequest>
    {
        public IEnumerable<SwaggerExample<CalculatorOperationRequest>> GetExamples()
        {
            CalculatorOperationRequest calcultorOperationRequest = new CalculatorOperationRequest
            {
                Operation = Operation.Sum,
                TypeNumbers = TypeNumber.Integer,
                Addends = new List<string> { "10", "20" }
            };

            yield return SwaggerExample.Create("IntegerSumOperationRequestExample", calcultorOperationRequest);
        }
    }
}
