using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using FDLM.Application.UseCases.Calculator;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos;
using Microsoft.Extensions.Logging;
using FDLM.Infrastructure.EntrypointsAdapters.Resources;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities.Examples;
using AutoMapper;
using FDLM.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Controllers
{
    [ApiController]
    [Route("v1/calculator")]
    [Produces("application/json")]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorUseCase _useCase;
        private readonly IRestTools _restTools;
        private readonly ITools _tools;
        private readonly IMapper _mapper;
        private readonly IInfraEntrypointsResourceService _resource;
        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(
            ICalculatorUseCase useCase,
            ITools tools,
            IRestTools restTools,
            IMapper mapper,
            IInfraEntrypointsResourceService resource, 
            ILogger<CalculatorController> logger)
        {
            _useCase = useCase;
            _tools = tools;
            _restTools = restTools;
            _mapper = mapper;
            _resource = resource;
            _logger = logger;
        }

        [HttpPost("sum")]
        [SwaggerOperation(Summary = "Permite sumar dos números", Description = "Permite sumar dos números enteros o complejos.")]
        [SwaggerRequestExample(typeof(CalculatorOperationRequest), typeof(IntegerSumOperationRequestExample))]
        [SwaggerRequestExample(typeof(CalculatorOperationRequest), typeof(ComplexSumOperationRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(IntegerSumOperationRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComplexSumOperationResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorOperationResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorOperationResponseExample))]
        [ProducesResponseType(typeof(Results<CalculatorOperationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Sum(CalculatorOperationRequest operationRequest)
        {
            var response = new Results<CalculatorOperationResponse>();
            try
            {
                IList<Number> addends = new List<Number>();
                switch (operationRequest.TypeNumbers)
                {
                    case TypeNumber.Integer:
                        {
                            addends = operationRequest.Addends.Select(a => (Number)new IntegerNumber(int.Parse(a))).ToList();
                            break;
                        }
                    case TypeNumber.Complex:
                        {
                            addends = operationRequest.Addends.Select(a => (Number)new ComplexNumber(int.Parse(a.Split("i")[0]), int.Parse(a.Split("i")[1]))).ToList();
                            break;
                        }
                    default: break;
                }

                var result = await _useCase.SumAndSaveOperationAsync(addends);
                if (result.IsSuccess)
                {
                    response.Result = _mapper.Map<CalculatorOperationResponse>(result.Result);
                    response.TotalItemsReturned = result.TotalItemsReturned;
                    response.TotalItemsInDataBase = result.TotalItemsInDataBase;
                }
                else
                {
                    response.AddErrors(result.Errors);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCode.CLIENT_ERROR, _resource.GetString(EntrypointResource.BadRequest));
                _logger.LogError(ex, _resource.GetString(EntrypointResource.BadRequest));
            }

            int statusCode = _restTools.GetHttpStatusCode(response.Errors);
            return StatusCode(statusCode, response);
        }

        [HttpGet("operation")]
        [SwaggerOperation(Summary = "Obtiene las operaciones almacenadas en base de datos", Description = "Permite consultar por un rango de fechas las operaciones realizadas por todos los usuarios.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetOperationsResponseExample))]
        [ProducesResponseType(typeof(Results<IList<CalculatorOperationResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOperations(
            [FromQuery, SwaggerParameter("Formato: dd-mm-aaaa hh:mm:ss, Ejemplo: 22-07-2024 15:00:00")] string startDate = "01-07-2024 15:00:00",
            [FromQuery, SwaggerParameter("Formato: dd-mm-aaaa hh:mm:ss, Ejemplo: 22-07-2024 15:00:00")] string endDate = "30-07-2024 15:00:00",
            [FromQuery, SwaggerParameter("Número mayor que cero que indica el número de página a obtener")] int pageNumber = 1,
            [FromQuery, SwaggerParameter("Número mayor que cero que indica la cantidad de registros a obtener")] int pageSize = 50,
            [FromQuery, SwaggerParameter("Campo de la base de datos por el cuál se ordenan los resultados")] string sortField = "CreationDate",
            [FromQuery, SwaggerParameter("Dirección de ordenamiento, ascendente o descendente: Asc, Desc")] SortDirection sortDirection = SortDirection.Asc)
        {
            var response = new Results<IList<CalculatorOperationResponse>>();
            try
            {   
                DateTime startDateUtc = _tools.DateTimeBogotaToUtc(startDate);
                DateTime endDateUtc = _tools.DateTimeBogotaToUtc(endDate);

                if (startDateUtc != DateTime.MinValue && endDateUtc != DateTime.MinValue)
                {
                    var result = await _useCase.GetOperationsByCreationDateAsync(startDateUtc, endDateUtc, pageNumber, pageSize, sortField, sortDirection);
                    if (result.IsSuccess)
                    {
                        response.Result = result.Result.Select(x => _mapper.Map<CalculatorOperationResponse>(x)).ToList();
                        response.TotalItemsReturned = result.TotalItemsReturned;
                        response.TotalItemsInDataBase = result.TotalItemsInDataBase;
                    }
                    else
                    {
                        response.AddErrors(result.Errors);
                    }
                }
                else
                {
                    string error = $"{_resource.GetString(EntrypointResource.DateFormatError)}Los valores ingresados fueron: {startDate} y {endDate}";
                    response.AddError(ErrorCode.CLIENT_ERROR, error);
                    _logger.LogError(error);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCode.CLIENT_ERROR, _resource.GetString(EntrypointResource.BadRequest));
                _logger.LogError(ex, _resource.GetString(EntrypointResource.BadRequest));
            }

            int statusCode = _restTools.GetHttpStatusCode(response.Errors);
            return StatusCode(statusCode, response);
        }

        [HttpGet("operation/{id}")]
        [SwaggerOperation(Summary = "Obtiene una operación por su ID", Description = "Permite consultar por ID una operación almacenada en la base de datos.")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetOperationByIdResponseExample))]
        [ProducesResponseType(typeof(Results<CalculatorOperationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Results<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOperation(string id)
        {
            var response = new Results<CalculatorOperationResponse>();
            try
            {
                var result = await _useCase.GetOperationAsync(id);
                if (result.IsSuccess)
                {
                    response.Result = _mapper.Map<CalculatorOperationResponse>(result.Result);
                    response.TotalItemsReturned = result.TotalItemsReturned;
                    response.TotalItemsInDataBase = result.TotalItemsInDataBase;
                }
                else
                {
                    response.AddErrors(result.Errors);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ErrorCode.CLIENT_ERROR, _resource.GetString(EntrypointResource.BadRequest));
                _logger.LogError(ex, _resource.GetString(EntrypointResource.BadRequest));
            }

            int statusCode = _restTools.GetHttpStatusCode(response.Errors);
            return StatusCode(statusCode, response);
        }
    }
}