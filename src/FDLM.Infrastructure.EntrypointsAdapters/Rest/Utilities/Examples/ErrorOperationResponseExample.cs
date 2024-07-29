using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using FDLM.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using FDLM.Domain.Models.Result;
using FDLM.Utilities;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples
{
    [ExcludeFromCodeCoverage]
    public class ErrorOperationResponseExample : IMultipleExamplesProvider<Results<CalculatorOperationResponse>>
    {
        public IEnumerable<SwaggerExample<Results<CalculatorOperationResponse>>> GetExamples()
        {
            CalculatorOperationResponse calcultorOperationResponse = new CalculatorOperationResponse
            {
                Operation = Operation.Sum,
                TypeNumbers = TypeNumber.Integer,
                Addends = new List<string> { "aaa", "10.2" },
            };

            Results<CalculatorOperationResponse> results = new Results<CalculatorOperationResponse>();
            results.Result = calcultorOperationResponse;
            results.AddError("Solo se permiten números enteros o complejos");

            yield return SwaggerExample.Create("ErrorOperationResponseExample", results);
        }
    }
}
