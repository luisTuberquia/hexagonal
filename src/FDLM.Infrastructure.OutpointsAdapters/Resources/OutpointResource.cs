using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Resources
{
    internal class OutpointResource
    {
        public readonly static string CouldNotSaveDocument = "CouldNotSaveDocument";
        public readonly static string DocumentNotFound = "DocumentNotFound";
        public readonly static string DynamoDBError = "DynamoDBError";
        public readonly static string LiteDBError = "LiteDBError";
    }
}
