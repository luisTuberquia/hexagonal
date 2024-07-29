using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Application.Resources
{
    internal interface IApplicationResourceService
    {
        string GetString(string name);
    }
}
