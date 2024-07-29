using FDLM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Dtos
{
    public class CalculatorOperationResponse
    {        
        public string Id { get; set; }
        public Operation Operation { get; set; }
        public TypeNumber TypeNumbers { get; set; }
        public List<string> Addends { get; set; }
        public string OperationResult { get; set; }
        public string CreationDate { get; set; }
    }
}
