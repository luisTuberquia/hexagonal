using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Config;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB
{
    internal class DynamoDbClient
    {
        private readonly ILogger<DynamoDbClient> _logger;
        public AmazonDynamoDBClient Client { get; }
        public DynamoDBContext Context { get; }

        public DynamoDbClient(IOptions<DynamoDbConfig> configs, ILogger<DynamoDbClient> logger)
        {
            _logger = logger;

            try
            {             
                var dynamoConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(configs.Value.Region),
                    ServiceURL = configs.Value.ServiceURL
                };

                Client = new AmazonDynamoDBClient(dynamoConfig);
                Context = new DynamoDBContext(Client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }        
    }
}
