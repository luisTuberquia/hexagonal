using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Models.Result
{
    public enum ErrorCode
    {
        SERVER_ERROR,
        CLIENT_ERROR,
        VALIDATION_ERROR,
        AUTHENTICATION_ERROR,
        AUTHORIZATION_ERROR,
        NOT_FOUND,
        GENERIC_ERROR
    }
}
