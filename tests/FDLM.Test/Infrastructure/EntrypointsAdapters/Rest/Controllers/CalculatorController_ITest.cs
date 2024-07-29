using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Test.Infrastructure.EntrypointsAdapters.Rest.Controllers
{
    public class CalculatorController_ITest : IClassFixture<WebAppTestFactory>
    {
        private readonly HttpClient _client;
        private readonly WebAppTestFactory _factory;

        public CalculatorController_ITest(WebAppTestFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Theory]
        [ClassData(typeof(PositiveTestData))]
        public async Task SumPositiveNumbers(CalculatorOperationRequest request, Results<CalculatorOperationResponse> expected)
        {
            // Given            
            var jsonRequest = JsonConvert.SerializeObject(request);
            var payload = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Execute
            var response = await _client.PostAsync("/v1/calculator/sum", payload);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Results<CalculatorOperationResponse> responseObject = JsonConvert.DeserializeObject<Results<CalculatorOperationResponse>>(responseString);
            Assert.True(responseObject.IsSuccess);
            Assert.Equal(expected.Result.OperationResult, responseObject.Result.OperationResult);
        }

        // ***************************************************************************************************** //
        // ******************************************* D A T A ***********-----********************************* //
        private class PositiveTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] 
                { 
                    new CalculatorOperationRequest { Operation = Operation.Sum, TypeNumbers = TypeNumber.Integer, Addends = new List<string>() { "10", "20" } },
                    new Results<CalculatorOperationResponse>(new CalculatorOperationResponse { Operation = Operation.Sum, OperationResult = "30" })
                },
                new object[]
                {
                    new CalculatorOperationRequest { Operation = Operation.Sum, TypeNumbers = TypeNumber.Integer, Addends = new List<string>() { "0", "0" } },
                    new Results<CalculatorOperationResponse>(new CalculatorOperationResponse { Operation = Operation.Sum, OperationResult = "0" })
                },
                new object[] 
                { 
                    new CalculatorOperationRequest { Operation = Operation.Sum, TypeNumbers = TypeNumber.Complex, Addends = new List<string>() { "10i5", "20i7" } },
                    new Results<CalculatorOperationResponse>(new CalculatorOperationResponse { Operation = Operation.Sum, OperationResult = "30i12" })
                },
                new object[] 
                { 
                    new CalculatorOperationRequest { Operation = Operation.Sum, TypeNumbers = TypeNumber.Complex, Addends = new List<string>() { "0i0", "0i0" } },
                    new Results<CalculatorOperationResponse>(new CalculatorOperationResponse { Operation = Operation.Sum, OperationResult = "0i0" })
                }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
