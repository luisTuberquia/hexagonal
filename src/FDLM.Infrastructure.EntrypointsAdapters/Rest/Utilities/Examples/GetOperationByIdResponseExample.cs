using FDLM.Utilities;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples
{
    [ExcludeFromCodeCoverage]
    public class GetOperationByIdResponseExample : IMultipleExamplesProvider<Results<CalculatorOperationResponse>>
    {
        public IEnumerable<SwaggerExample<Results<CalculatorOperationResponse>>> GetExamples()
        {
            ITools tools = new Tools();            

            CalculatorOperationResponse operation = new CalculatorOperationResponse()
            {                
                Id = $"{Operation.Sum}-{tools.ToUnixEpoch(DateTime.Now)}",
                Operation = Operation.Sum,
                TypeNumbers = TypeNumber.Complex,
                Addends = new List<string> { "10i5", "20i7" },
                OperationResult = "30i12",
                CreationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
            };
            
            Results<CalculatorOperationResponse> results = new Results<CalculatorOperationResponse>();
            results.Result = operation;

            yield return SwaggerExample.Create("GetOperationByIdResponseExample", results);
        }
    }
}
