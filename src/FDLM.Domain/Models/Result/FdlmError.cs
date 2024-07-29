using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FDLM.Domain.Models.Result
{
    public class FdlmError
    {
        public string Message { get; private set; }
        public ErrorCode ErrorCode { get; private set; }

        [JsonIgnore]
        public Exception Exception { get; private set; }

        public FdlmError(ErrorCode code, String message, Exception exception)
        {
            this.ErrorCode = code;
            this.Message = message;
            this.Exception = exception;
        }

        public FdlmError(ErrorCode code, String message)
        {
            this.ErrorCode = code;
            this.Message = message;
            this.Exception = new Exception(message);
        }
    }
}
