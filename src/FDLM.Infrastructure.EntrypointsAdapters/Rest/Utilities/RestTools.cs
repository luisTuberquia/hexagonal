using FDLM.Domain.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities
{
    internal class RestTools : IRestTools
    {
        public int GetHttpStatusCode(List<FdlmError> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return 200; 
            }

            return errors.Select(e =>
                    e.ErrorCode == ErrorCode.AUTHENTICATION_ERROR ? 401
                    : e.ErrorCode == ErrorCode.AUTHORIZATION_ERROR ? 403
                    : e.ErrorCode == ErrorCode.NOT_FOUND ? 404
                    : e.ErrorCode == ErrorCode.CLIENT_ERROR || e.ErrorCode == ErrorCode.VALIDATION_ERROR ? 400
                    : 500).First();
        }
    }
}
