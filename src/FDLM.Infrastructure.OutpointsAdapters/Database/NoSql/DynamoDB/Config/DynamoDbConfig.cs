using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Config
{
    internal class DynamoDbConfig
    {
        public string Region { get; set; }
        public string ServiceURL{ get; set; }
        public int CacheDurationInMinutesForPaging { get; set; }
    }
}
