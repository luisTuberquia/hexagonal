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
    public class GetOperationsResponseExample : IMultipleExamplesProvider<Results<IList<CalculatorOperationResponse>>>
    {
        public IEnumerable<SwaggerExample<Results<IList<CalculatorOperationResponse>>>> GetExamples()
        {
            ITools tools = new Tools();            

            IList<CalculatorOperationResponse> operations = new List<CalculatorOperationResponse>()
            {
                new CalculatorOperationResponse
                {
                    Id = $"{Operation.Sum}-{tools.ToUnixEpoch(DateTime.Now)}",
                    Operation = Operation.Sum,
                    TypeNumbers = TypeNumber.Complex,
                    Addends = new List<string> { "10i5", "20i7" },
                    OperationResult = "30i12",
                    CreationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),

                },
                new CalculatorOperationResponse
                {
                    Id = $"{Operation.Sum}-{tools.ToUnixEpoch(DateTime.Now) + 10}",
                    Operation = Operation.Sum,
                    TypeNumbers = TypeNumber.Integer,
                    Addends = new List<string> { "10", "20" },
                    OperationResult = "30",
                    CreationDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),

                }

            };
            
            Results<IList<CalculatorOperationResponse>> results = new Results<IList<CalculatorOperationResponse>>();
            results.Result = operations;

            yield return SwaggerExample.Create("GetOperationsResponseExample", results);
        }
    }
}
