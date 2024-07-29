﻿using FDLM.Application.Resources;
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
    public class ComplexOperationService_ITest
    {
        private Mock<ILogger<ComplexOperationService>> _loggerMock;
        private IDataValidator _dataValidator;
        private IDomainResourceService _resource;

        public ComplexOperationService_ITest() 
        {
            // Mocks Configuration
            _loggerMock = new Mock<ILogger<ComplexOperationService>>();
            var validationLoggerMock = new Mock<ILogger<DataValidatorService>>();

            // Dependency Injector Configuration
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDomainResourceService, DomainResourceService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _resource = serviceProvider.GetService<IDomainResourceService>();
                        
            _dataValidator = new DataValidatorService(_resource, validationLoggerMock.Object);
        }

        [Theory]
        [ClassData(typeof(PositiveComplexTestData))]
        public async Task SumTwoComplexPositiveNumbers(IList<Number> addends, Results<Number> expected)
        {
            //Given
            IOperation calculator = new ComplexOperationService(_dataValidator, _loggerMock.Object);            

            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.Equal((int)expected.Result.Value, (int)result.Result.Value);
        }
                

        [Theory]
        [ClassData(typeof(NegativeComplexTestData))]
        public async Task SumNegativeComplexNumbers(IList<Number> addends)
        {
            //Given
            IOperation calculator = new ComplexOperationService(_dataValidator, _loggerMock.Object);            

            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(_resource.GetString(DomainResource.NumbersAreNegative), result.Errors.First().Message);            
        }
        
        [Theory]
        [ClassData(typeof(NullComplexTestData))]
        public async Task SumNullComplexNumbers(IList<Number> addends)
        {
            //Given
            IOperation calculator = new ComplexOperationService(_dataValidator, _loggerMock.Object);            

            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(_resource.GetString(DomainResource.NotAllowNullValues), result.Errors.First().Message);
        }


        // ***************************************************************************************************** //
        // ******************************************* D A T A ***********-----********************************* //
        private class PositiveComplexTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new ComplexNumber(10, 200), new ComplexNumber(20, 10) }, new Results<Number>(new ComplexNumber(30, 210)) },
                new object[] { new List<Number>() { new ComplexNumber(0, 0), new ComplexNumber(0, 0) }, new Results<Number>(new ComplexNumber(0, 0)) },
                new object[] { new List<Number>() { new ComplexNumber(0, 10), new ComplexNumber(400, 10) }, new Results<Number>(new ComplexNumber(400, 20)) }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NegativeComplexTestData : IEnumerable<object[]>
        {            
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new ComplexNumber(-20, 10), new ComplexNumber(60, 100) } },
                new object[] { new List<Number>() { new ComplexNumber(0, -10), new ComplexNumber(-20, 200) } },
                new object[] { new List<Number>() { new ComplexNumber(-20, -50), new ComplexNumber(-20, 300) } }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NullComplexTestData : IEnumerable<object[]>
        {            
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { null, new ComplexNumber(20, 5) } },
                new object[] { new List<Number>() { new ComplexNumber(0, 10), null } },
                new object[] { new List<Number>() { null, null } },
                new object[] { null }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}