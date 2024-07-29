using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Resources
{
    internal interface IInfraOutpointsResourceService
    {
        string GetString(string name);
    }
}
