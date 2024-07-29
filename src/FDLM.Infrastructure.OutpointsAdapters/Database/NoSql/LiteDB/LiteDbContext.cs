using Amazon.Runtime.Internal.Util;
using LiteDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FDLM.Domain.Models;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Documents;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Config;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB
{
    internal class LiteDbContext
    {
        public LiteDatabase Context { get; }
        private readonly ILogger<LiteDbContext> _logger;

        public LiteDbContext(IOptions<LiteDbConfig> configs, ILogger<LiteDbContext> logger)
        {
            _logger = logger;
            try
            {
                Context = new LiteDatabase(configs.Value.Path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        public ILiteCollection<CalculatorOperationDocument> CalculatorOperations => Context.GetCollection<CalculatorOperationDocument>("CalculatorOperations");
    }
}
