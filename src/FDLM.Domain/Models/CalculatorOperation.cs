using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models
{
    public class CalculatorOperation
    {
        public string Id { get; set; }
        public Operation Operation { get; set; }
        public TypeNumber TypeNumbers { get; set; }
        public List<string> Addends { get; set; }
        public string OperationResult { get; set; } = string.Empty;
        public DateTime CreationDateUtc { get; set; }
    }
}
