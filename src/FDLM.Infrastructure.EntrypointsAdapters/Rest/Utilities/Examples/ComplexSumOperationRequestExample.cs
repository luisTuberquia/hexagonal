using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using FDLM.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples
{
    [ExcludeFromCodeCoverage]
    public class ComplexSumOperationRequestExample : IMultipleExamplesProvider<CalculatorOperationRequest>
    {
        public IEnumerable<SwaggerExample<CalculatorOperationRequest>> GetExamples()
        {
            CalculatorOperationRequest calcultorOperationRequest = new CalculatorOperationRequest
            {
                Operation = Operation.Sum,
                TypeNumbers = TypeNumber.Complex,
                Addends = new List<string> { "10i5", "20i30" }
            };

            yield return SwaggerExample.Create("ComplexSumOperationRequestExample", calcultorOperationRequest);
        }
    }
}
