using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Polly;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Application.Ports;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB;
using System;
using System.Collections.Generic;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Tables;
using FDLM.Utilities;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Config;
using System.Collections.Concurrent;
using System.Globalization;
using FDLM.Infrastructure.OutpointsAdapters.Resources;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Repositories
{
    internal class DynamoCalculatorOperationRepositoryAdapter : ICalculatorOperationRepositoryPort
    {
        private readonly DynamoDbClient _dynamo;
        private readonly IMapper _mapper;
        private readonly IInfraOutpointsResourceService _resource;
        private readonly ITools _tools;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<DynamoDbConfig> _config;
        private readonly IAsyncPolicy _circuitBreaker;
        private readonly ILogger<DynamoCalculatorOperationRepositoryAdapter> _logger;

        public DynamoCalculatorOperationRepositoryAdapter(
            DynamoDbClient dynamo,
            IMapper mapper,
            IInfraOutpointsResourceService resource,
            ITools tools,
            IMemoryCache memoryCache,
            IOptions<DynamoDbConfig> config,
            IAsyncPolicy circuitBreaker,
            ILogger<DynamoCalculatorOperationRepositoryAdapter> logger)
        {
            _dynamo = dynamo;
            _mapper = mapper;
            _resource = resource;
            _tools = tools;
            _memoryCache = memoryCache;
            _config = config;
            _circuitBreaker = circuitBreaker;
            _logger = logger;
        }

        #region Public Methods
        public async Task<Results<CalculatorOperation>> SaveAsync(CalculatorOperation model)
        {
            var results = new Results<CalculatorOperation>();
            try
            {
                await _circuitBreaker.ExecuteAsync(() => _dynamo.Context.SaveAsync(_mapper.Map<CalculatorOperationTable>(model)));
                results.TotalItemsInDataBase = 1;
                results.TotalItemsReturned = 1;
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.DynamoDBError)}Model: {model}";
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
                string partitionKey;
                long sortKey;
                GetIndexedKeys(id, out partitionKey, out sortKey);

                var response = await _circuitBreaker.ExecuteAsync(() => _dynamo.Context.LoadAsync<CalculatorOperationTable>(partitionKey, sortKey));            
                if (response == null)
                {
                    results.AddError(new FdlmError(ErrorCode.NOT_FOUND, $"{_resource.GetString(OutpointResource.DocumentNotFound)}Id: {id}"));
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
                var error = $"{_resource.GetString(OutpointResource.DynamoDBError)}Id: {id}";
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
                long startDateUnixEpoch = _tools.ToUnixEpoch(startDateUtc);
                long endDateUnixEpoch = _tools.ToUnixEpoch(endDateUtc);

                results.Result = await GetItemsOfPage(startDateUnixEpoch, endDateUnixEpoch, pageNumber, pageSize, sortField, sortDirection);

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
                var error = $"{_resource.GetString(OutpointResource.DynamoDBError)}startDateUtc: {startDateUtc}, endDateUtc: {endDateUtc}, pageNumber: {pageNumber}, pageSize: {pageSize}";
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
                long startDateUnixEpoch = _tools.ToUnixEpoch(startDateUtc);
                long endDateUnixEpoch = _tools.ToUnixEpoch(endDateUtc);

                var queryRequest = new QueryRequest
                {
                    TableName = "CalculatorOperations",                                       
                    KeyConditionExpression = "#operation = :operation AND #creationDateEpochUnix BETWEEN :startDateUnixEpoch AND :endDateUnixEpoch",
                    Select = "COUNT", // Solo obtener el conteo de ítems
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#operation", "Operation"},
                        {"#creationDateEpochUnix", "CreationDateEpochUnix"}
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":operation", new AttributeValue { S = "Sum" }},
                        {":startDateUnixEpoch", new AttributeValue { N = startDateUnixEpoch.ToString() }},
                        {":endDateUnixEpoch", new AttributeValue { N = endDateUnixEpoch.ToString() }}
                    }
                };

                var queryResponse = await _dynamo.Client.QueryAsync(queryRequest);
                results.Result = queryResponse is not null ? queryResponse.Count : 0;
                results.TotalItemsInDataBase = results.Result > 0 ? 1 : 0;
                results.TotalItemsReturned = results.Result > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                var error = $"{_resource.GetString(OutpointResource.DynamoDBError)}startDateUtc: {startDateUtc}, endDateUtc: {endDateUtc}";
                results.AddError(ErrorCode.SERVER_ERROR, error);
                _logger.LogError(ex, error);
            }
            return results;
        }
        #endregion

        #region Private Methods
        private void GetIndexedKeys(string id, out string partitionKey, out long sortKey)
        {
            partitionKey = null;
            sortKey = 0;

            var parts = id.Split("-");
            if (parts.Length == 2)
            {
                partitionKey = parts[0];
                sortKey = long.Parse(parts[1]);
            }
        }

        private async Task<IList<CalculatorOperation>> GetItemsOfPage(long startDateUnixEpoch, long endDateUnixEpoch, int pageNumber, int pageSize, string sortField, SortDirection sortDirection)
        {
            QueryResponse queryResponse = await AdvanceToPageNumber(startDateUnixEpoch, endDateUnixEpoch, pageNumber, pageSize, sortField, sortDirection);
            IList<CalculatorOperation> items = GetCalculatorOperationItems(queryResponse);

            return items;
        }

        private async Task<QueryResponse> AdvanceToPageNumber(long startDateUnixEpoch, long endDateUnixEpoch, int pageNumber, int pageSize, string sortField, SortDirection sortDirection)
        {
            string key = $"{pageNumber}-{pageSize}-{startDateUnixEpoch}-{endDateUnixEpoch}";
            Dictionary<string, AttributeValue> lastEvaluatedKey = GetLastEvaluatedKeyFromCache(key);

            QueryResponse queryResponse = null;
            for (int currentPage = 1; currentPage <= pageNumber; currentPage++)
            {
                var queryRequest = GetQueryRequest(startDateUnixEpoch, endDateUnixEpoch, pageSize, sortField, sortDirection, lastEvaluatedKey);
                queryResponse = await _circuitBreaker.ExecuteAsync(() => _dynamo.Client.QueryAsync(queryRequest));

                lastEvaluatedKey = queryResponse.LastEvaluatedKey;
                if (currentPage == pageNumber || lastEvaluatedKey == null || lastEvaluatedKey.Count == 0)
                {
                    break;
                }
            }

            _memoryCache.Set(key, lastEvaluatedKey, TimeSpan.FromMinutes(_config.Value.CacheDurationInMinutesForPaging));

            return queryResponse;
        }

        private Dictionary<string, AttributeValue> GetLastEvaluatedKeyFromCache(string key)
        {
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;
            if (_memoryCache.TryGetValue(key, out Dictionary<string, AttributeValue> lastEvaluatedKeyCached))
            {
                lastEvaluatedKey = lastEvaluatedKeyCached;
            }

            return lastEvaluatedKey;
        }

        private QueryRequest GetQueryRequest(long startDateUnixEpoch, long endDateUnixEpoch, int pageSize, string sortField, SortDirection sortDirection, Dictionary<string, AttributeValue> lastEvaluatedKey)
        {
            return new QueryRequest
            {
                TableName = "CalculatorOperations",
                Limit = pageSize,
                ExclusiveStartKey = lastEvaluatedKey,
                KeyConditionExpression = "#operation = :operation AND #creationDateEpochUnix BETWEEN :startDateUnixEpoch AND :endDateUnixEpoch",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#operation", "Operation"},
                    {"#creationDateEpochUnix", "CreationDateEpochUnix"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":operation", new AttributeValue { S = "Sum" }},
                    {":startDateUnixEpoch", new AttributeValue { N = startDateUnixEpoch.ToString() }},
                    {":endDateUnixEpoch", new AttributeValue { N = endDateUnixEpoch.ToString() }}
                },
                ScanIndexForward = sortDirection == SortDirection.Asc
            };
        }

        private IList<CalculatorOperation> GetCalculatorOperationItems(QueryResponse queryResponse)
        {
            IList<CalculatorOperation> items = new List<CalculatorOperation>();

            if (queryResponse != null)
            {
                foreach (var item in queryResponse.Items)
                {
                    TypeNumber typeNumbers = Enum.TryParse(item["TypeNumbers"].N, out typeNumbers) ? typeNumbers : TypeNumber.Unknow;
                    Operation operation = Enum.TryParse(item["Operation"].S, out operation) ? operation : Operation.Unknow;
                    long creationDateEpochUnix = long.TryParse(item["CreationDateEpochUnix"].N, out creationDateEpochUnix) ? creationDateEpochUnix : 0;

                    var calculatorOperation = new CalculatorOperation
                    {
                        Id = $"{operation}-{creationDateEpochUnix}",
                        Operation = operation,
                        TypeNumbers = typeNumbers,
                        OperationResult = item["OperationResult"].S,
                        Addends = item["Addends"].SS,
                        CreationDateUtc = _tools.ToDateTime(creationDateEpochUnix)
                    };

                    items.Add(calculatorOperation);
                }
            }

            return items;
        }
        #endregion
    }
}
