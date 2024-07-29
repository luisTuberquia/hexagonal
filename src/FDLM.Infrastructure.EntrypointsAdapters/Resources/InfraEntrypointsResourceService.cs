using FDLM.Infrastructure.EntrypointsAdapters.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.EntrypointsAdapters.Resources
{
    internal class InfraEntrypointsResourceService : IInfraEntrypointsResourceService
    {
        private readonly ResourceManager _resourceManager;

        public InfraEntrypointsResourceService()
        {            
            _resourceManager = new ResourceManager("FDLM.Infrastructure.EntrypointsAdapters.Resources.Resources", typeof(InfraEntrypointsResourceService).Assembly);
        }

        public string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}
