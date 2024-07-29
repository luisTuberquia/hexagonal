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
    public class IntegerOperationService_UTest
    {
        private Mock<ILogger<IntegerOperationService>> _loggerMock;
        private Mock<IDataValidator> _dataValidatorMock;
        private IDomainResourceService _resource;

        public IntegerOperationService_UTest() 
        {
            _loggerMock = new Mock<ILogger<IntegerOperationService>>();

            // Dependency Injector Configuration
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDomainResourceService, DomainResourceService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _resource = serviceProvider.GetService<IDomainResourceService>();
        }

        [Theory]
        [ClassData(typeof(PositiveIntegersTestData))]
        public async Task SumTwoIntegerPositiveNumbers(IList<Number> addends, Results<Number> expected)
        {
            //Mock Configurations            
            _dataValidatorMock = new Mock<IDataValidator>();
            _dataValidatorMock
                .Setup(service => service.InputsAreNotNull(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));
            _dataValidatorMock
                .Setup(service => service.NumbersArePositive(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));
            _dataValidatorMock
                .Setup(service => service.InputsAreIntegerNumbers(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));

            //Given
            IOperation calculator = new IntegerOperationService(_dataValidatorMock.Object, _loggerMock.Object);
            
            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.Equal((int)expected.Result.Value, (int)result.Result.Value);
            _dataValidatorMock.Verify(service => service.InputsAreNotNull(It.IsAny<IList<Number>>()), Times.Once);
            _dataValidatorMock.Verify(service => service.NumbersArePositive(It.IsAny<IList<Number>>()), Times.Once);
            _dataValidatorMock.Verify(service => service.InputsAreIntegerNumbers(It.IsAny<IList<Number>>()), Times.Once);
        }
                

        [Theory]
        [ClassData(typeof(NegativeIntegersTestData))]
        public async Task SumNegativeIntegerNumbers(IList<Number> addends)
        {
            //Mock Configurations            
            _dataValidatorMock = new Mock<IDataValidator>();
            _dataValidatorMock
                .Setup(service => service.InputsAreNotNull(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));
            _dataValidatorMock
                .Setup(service => service.NumbersArePositive(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(false).AddError(_resource.GetString(DomainResource.NumbersAreNegative)));
            _dataValidatorMock
                .Setup(service => service.InputsAreIntegerNumbers(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));

            //Given
            IOperation calculator = new IntegerOperationService(_dataValidatorMock.Object, _loggerMock.Object);
            
            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(_resource.GetString(DomainResource.NumbersAreNegative), result.Errors.First().Message);
            _dataValidatorMock.Verify(service => service.NumbersArePositive(It.IsAny<IList<Number>>()), Times.Once);
        }
        
        [Theory]
        [ClassData(typeof(NullIntegersTestData))]
        public async Task SumNullIntegerNumbers(IList<Number> addends)
        {
            //Mock Configurations            
            _dataValidatorMock = new Mock<IDataValidator>();
            _dataValidatorMock
                .Setup(service => service.InputsAreNotNull(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(false).AddError(_resource.GetString(DomainResource.NotAllowNullValues)));
            _dataValidatorMock
                .Setup(service => service.NumbersArePositive(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));
            _dataValidatorMock
                .Setup(service => service.InputsAreIntegerNumbers(It.IsAny<IList<Number>>()))
                .Returns((IList<Number> addends) => new Results<bool>(true));

            //Given
            IOperation calculator = new IntegerOperationService(_dataValidatorMock.Object, _loggerMock.Object);
            
            //Execute
            var result = await calculator.SumAsync(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.Equal(_resource.GetString(DomainResource.NotAllowNullValues), result.Errors.First().Message);
            _dataValidatorMock.Verify(service => service.InputsAreNotNull(It.IsAny<IList<Number>>()), Times.Once);
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
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(-20), new IntegerNumber(20) } },
                new object[] { new List<Number>() { new IntegerNumber(0), new IntegerNumber(-20) } },
                new object[] { new List<Number>() { new IntegerNumber(-20), new IntegerNumber(-20) } }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NullIntegersTestData : IEnumerable<object[]>
        {            
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { null, new IntegerNumber(20) } },
                new object[] { new List<Number>() { new IntegerNumber(0), null } },
                new object[] { new List<Number>() { null, null } },
                new object[] { null }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }        
    }
}
