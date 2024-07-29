using Amazon.DynamoDBv2.DataModel;
using FDLM.Domain.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Documents
{    
    internal class CalculatorOperationDocument
    {
        [BsonId]
        public string Id { get; set; }
        public string Operation { get; set; }
        public long CreationDateEpochUnix { get; set; }
        public TypeNumber TypeNumbers { get; set; }
        public string OperationResult { get; set; } = string.Empty;
        public List<string> Addends { get; set; }
    }
}
