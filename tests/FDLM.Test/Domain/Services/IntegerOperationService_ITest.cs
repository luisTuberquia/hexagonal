using FDLM.Application.Resources;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Domain.Resources;
using FDLM.Domain.Services;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Test.Domain.Services
{
    public class IntegerOperationService_ITest
    {
        private Mock<ILogger<IntegerOperationService>> _loggerMock;
        private IDataValidator _dataValidator;
        private static IDomainResourceService _resource;

        public IntegerOperationService_ITest() 
        {
            // Mocks Configuration
            _loggerMock = new Mock<ILogger<IntegerOperationService>>();
            var validationLoggerMock = new Mock<ILogger<DataValidatorService>>();

            // Dependency Injector Configuration
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDomainResourceService, DomainResourceService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _resource = serviceProvider.GetService<IDomainResourceService>();

            _dataValidator = new DataValidatorService(_resource, validationLoggerMock.Object);
        }

        [Theory]
        [ClassData(typeof(PositiveIntegersTestData))]
        public async Task SumTwoIntegerPositiveNumbers(IList<Number> addends, Results<Number> expected)
        {
            //Given
            IOperation calculator = new IntegerOperationService(_dataValidator, _loggerMock.Object);            

            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.Equal((int)expected.Result.Value, (int)result.Result.Value);
        }
                

        [Theory]
        [ClassData(typeof(NegativeIntegersTestData))]
        public async Task SumNegativeIntegerNumbers(IList<Number> addends, Results<Number> expected)
        {
            //Given
            IOperation calculator = new IntegerOperationService(_dataValidator, _loggerMock.Object);
            
            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(expected.Errors.First().Message, result.Errors.First().Message);            
        }
        
        [Theory]
        [ClassData(typeof(NullIntegersTestData))]
        public async Task SumNullIntegerNumbers(IList<Number> addends, Results<Number> expected)
        {
            //Given
            IOperation calculator = new IntegerOperationService(_dataValidator, _loggerMock.Object);
            
            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(expected.Errors.First().Message, result.Errors.First().Message);
        }
                

        // ***************************************************************************************************** //
        // ******************************************* D A T A ***********-----********************************* //
        private class PositiveIntegersTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(10), new IntegerNumber(20) }, new Results<Number>(new IntegerNumber(30)) },
                new object[] { new List<Number>() { new IntegerNumber(0), new IntegerNumber(0) }, new Results<Number>(new IntegerNumber(0)) },
                new object[] { new List<Number>() { new IntegerNumber(0), new IntegerNumber(400) }, new Results<Number>(new IntegerNumber(400)) }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NegativeIntegersTestData : IEnumerable<object[]>
        {
            static string Message = _resource.GetString(DomainResource.NumbersAreNegative);
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(-20), new IntegerNumber(20) }, new Results<Number>().AddError(Message) },
                new object[] { new List<Number>() { new IntegerNumber(0), new IntegerNumber(-20) }, new Results<Number>().AddError(Message) },
                new object[] { new List<Number>() { new IntegerNumber(-20), new IntegerNumber(-20) }, new Results<Number>().AddError(Message) }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NullIntegersTestData : IEnumerable<object[]>
        {
            static string Message = _resource.GetString(DomainResource.NotAllowNullValues);
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { null, new IntegerNumber(20) }, new Results<Number>().AddError(Message) },
                new object[] { new List<Number>() { new IntegerNumber(0), null }, new Results<Number>().AddError(Message) },
                new object[] { new List<Number>() { null, null }, new Results<Number>().AddError(Message) },
                new object[] { null, new Results<Number>().AddError(Message) }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }        
    }
}
