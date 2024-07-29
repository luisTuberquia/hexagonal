using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FDLM.Domain.Models;
using LiteDB;
using FDLM.Domain.Models.Result;
using FDLM.Application.Ports;
using FDLM.Utilities;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Documents;
using System;
using System.Collections.Generic;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB;
using FDLM.Infrastructure.OutpointsAdapters.Resources;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Repositories;
using Microsoft.Extensions.Logging;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Repositories
{
    internal class LiteCalculatorOperationRepositoryAdapter : ICalculatorOperationRepositoryPort
    {
        private readonly LiteDbContext _context;
        private readonly ITools _tools;
        private readonly IMapper _mapper;
        private readonly IInfraOutpointsResourceService _resource;
        private readonly ILogger<LiteCalculatorOperationRepositoryAdapter> _logger;

        public LiteCalculatorOperationRepositoryAdapter(
            LiteDbContext context, 
            ITools tools, 
            IMapper mapper, 
            IInfraOutpointsResourceService resource,
            ILogger<LiteCalculatorOperationRepositoryAdapter> logger)
        {
            _context = context;
            _tools = tools;
            _mapper = mapper;
            _resource = resource;
            _logger = logger;
        }

        public async Task<Results<CalculatorOperation>> SaveAsync(CalculatorOperation model)
        {
            var results = new Results<CalculatorOperation>();
            try
            {
                var response = _context.CalculatorOperations.Insert(_mapper.Map<CalculatorOperationDocument>(model));
                if (response == null)
                {
                    results.AddError(ErrorCode.SERVER_ERROR, _resource.GetString(OutpointResource.CouldNotSaveDocument));
                }
                else
                {
                    results.TotalItemsInDataBase = 1;
                    results.TotalItemsReturned = 1;
                }
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.LiteDBError)}Model:{model}";
                results.AddError(ErrorCode.SERVER_ERROR, error);
                _logger.LogError(ex, error);
            }

            results.Result = model;            
            return results;
        }

        public async Task<Results<CalculatorOperation>> FindByIdAsync(string id)
        {
            var results = new Results<CalculatorOperation>();
            try
            {
                var response = _context.CalculatorOperations.FindById(id);
                if (response == null)
                {
                    results.AddError(new FdlmError(ErrorCode.NOT_FOUND, $"{_resource.GetString(OutpointResource.DocumentNotFound)}{id}"));
                }
                else
                {
                    results.Result = _mapper.Map<CalculatorOperation>(response);
                    results.TotalItemsInDataBase = 1;
                    results.TotalItemsReturned = 1;
                }
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.LiteDBError)}Id:{id}";
                results.AddError(ErrorCode.SERVER_ERROR, error);
                _logger.LogError(ex, error);
            }
            return results;
        }

        public async Task<Results<IList<CalculatorOperation>>> FindByCreationDateAsync(DateTime startDateUtc, DateTime endDateUtc, int pageNumber, int pageSize, string sortField, SortDirection sortDirection)
        {
            Results<IList<CalculatorOperation>> results = new Results<IList<CalculatorOperation>>();
            try
            {
                var items = new List<CalculatorOperation>();

                var startDateUnixEpoch = _tools.ToUnixEpoch(startDateUtc);
                var endDateUnixEpoch = _tools.ToUnixEpoch(endDateUtc);

                List<CalculatorOperationDocument> response = _context.CalculatorOperations
                    .Find(x => x.CreationDateEpochUnix >= startDateUnixEpoch && x.CreationDateEpochUnix <= endDateUnixEpoch)
                    .OrderBy(x => x.CreationDateEpochUnix)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                if (response != null)
                {
                    if (SortDirection.Desc == sortDirection)
                    {
                        response.Reverse();
                    }

                    items = response.Select(x => _mapper.Map<CalculatorOperation>(x)).ToList();
                }

                results.Result = items;

                var countResult = await Count(startDateUtc, endDateUtc);
                if (countResult.IsSuccess)
                {
                    results.TotalItemsInDataBase = countResult.Result;
                    results.TotalItemsReturned = results.Result is not null ? results.Result.Count : 0;
                }
                else
                {
                    results.AddErrors(countResult.Errors);
                }                
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.LiteDBError)}startDateUtc:{startDateUtc}, endDateUtc:{endDateUtc}, pageNumber:{pageNumber}, pageSize:{pageSize}";
                results.AddError(ErrorCode.SERVER_ERROR, error);
                _logger.LogError(ex, error);
            }
            return results;
        }

        public async Task<Results<long>> Count(DateTime startDateUtc, DateTime endDateUtc)
        {
            Results<long> results = new Results<long>();
            try
            {
                var startDateUnixEpoch = _tools.ToUnixEpoch(startDateUtc);
                var endDateUnixEpoch = _tools.ToUnixEpoch(endDateUtc);

                var query = Query.Between("CreationDateEpochUnix", startDateUnixEpoch, endDateUnixEpoch);
                var count = _context.CalculatorOperations.Count(query);

                results.Result = count;
                results.TotalItemsInDataBase = results.Result > 0 ? 1 : 0;
                results.TotalItemsReturned = results.Result > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.LiteDBError)}startDateUtc:{startDateUtc}, endDateUtc:{endDateUtc}";
                results.AddError(ErrorCode.SERVER_ERROR, error);
                _logger.LogError(ex, error);
            }
            return results;
        }
    }
}
