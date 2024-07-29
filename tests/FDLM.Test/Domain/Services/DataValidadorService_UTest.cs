using FDLM.Application.Resources;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Domain.Resources;
using FDLM.Domain.Services;
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
    public class DataValidadorService_UTest
    {
        private IDomainResourceService _resource;
        private Mock<ILogger<DataValidatorService>> _loggerMock;

        public DataValidadorService_UTest()
        {
            // Mocks Configuration
            _loggerMock = new Mock<ILogger<DataValidatorService>>();

            // Dependency Injector Configuration
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDomainResourceService, DomainResourceService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _resource = serviceProvider.GetService<IDomainResourceService>();
        }

        [Theory]
        [ClassData(typeof(NotNullNumbersTestData))]
        public void NumberInputsAreNotNull(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreNotNull(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.True(result.Result);
        }

        [Theory]
        [ClassData(typeof(NullNumbersTestData))]
        public void NumberInputsAreNull(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreNotNull(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.False(result.Result);
            Assert.Equal(_resource.GetString(DomainResource.NotAllowNullValues), result.Errors.First().Message);
        }

        [Theory]
        [ClassData(typeof(InputsAreIntegerNumbersTestData))]
        public void InputsAreIntegerNumbers(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreIntegerNumbers(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.True(result.Result);            
        }

        [Theory]
        [ClassData(typeof(InputsAreComplexNumbersTestData))]
        public void InputsMustBeIntegerNumbersButAreComplexNumbers(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreIntegerNumbers(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.False(result.Result);
            Assert.Equal(_resource.GetString(DomainResource.OnlyIntegerNumbers), result.Errors.First().Message);
        }

        [Theory]
        [ClassData(typeof(InputsAreComplexNumbersTestData))]
        public void InputsAreComplexNumbers(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreComplexNumbers(addends);

            //Then
            Assert.True(result.IsSuccess);
            Assert.True(result.Result);            
        }

        [Theory]
        [ClassData(typeof(InputsAreIntegerNumbersTestData))]
        public void InputsMustBeComplexNumbersButAreIntegerNumbers(IList<Number> addends)
        {
            //Given
            IDataValidator validador = new DataValidatorService(_resource, _loggerMock.Object);

            //Execute
            var result = validador.InputsAreComplexNumbers(addends);

            //Then
            Assert.True(result.HasErrors);
            Assert.False(result.Result);
            Assert.Equal(_resource.GetString(DomainResource.OnlyComplexNumbers), result.Errors.First().Message);
        }



        // ***************************************************************************************************** //
        // ******************************************* D A T A ***********-----********************************* //
        private class NotNullNumbersTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(10), new ComplexNumber(20, 0) } }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NullNumbersTestData : IEnumerable<object[]>
        {            
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(10), null } },
                new object[] { new List<Number>() { null, new IntegerNumber(10) } },
                new object[] { null }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class InputsAreIntegerNumbersTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new IntegerNumber(10), new IntegerNumber(20) } },
                new object[] { new List<Number>() { new IntegerNumber(-10), new IntegerNumber(0) } }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class InputsAreComplexNumbersTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { new List<Number>() { new ComplexNumber(10, 0), new ComplexNumber(20, 10) } },
                new object[] { new List<Number>() { new ComplexNumber(-10, 6), new ComplexNumber(0, 0) } }
            };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }        
    }
}
