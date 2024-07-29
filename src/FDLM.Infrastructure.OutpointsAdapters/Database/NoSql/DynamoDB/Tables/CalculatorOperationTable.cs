using Amazon.DynamoDBv2.DataModel;
using FDLM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Tables
{
    [DynamoDBTable("CalculatorOperations")]
    internal class CalculatorOperationTable
    {
        [DynamoDBHashKey]
        public string Operation { get; set; }

        [DynamoDBRangeKey]
        public long CreationDateEpochUnix { get; set; }

        [DynamoDBProperty]
        public TypeNumber TypeNumbers { get; set; }

        [DynamoDBProperty]
        public string OperationResult { get; set; } = string.Empty;

        [DynamoDBProperty]
        public List<string> Addends { get; set; }
    }
}
