using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Resources
{
    public interface IInfraEntrypointsResourceService
    {
        string GetString(string name);
    }
}
