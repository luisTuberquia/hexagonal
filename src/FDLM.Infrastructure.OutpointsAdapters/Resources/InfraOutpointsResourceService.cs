using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Infrastructure.OutpointsAdapters.Resources
{
    internal class InfraOutpointsResourceService : IInfraOutpointsResourceService
    {
        private readonly ResourceManager _resourceManager;

        public InfraOutpointsResourceService()
        {            
            _resourceManager = new ResourceManager("FDLM.Infrastructure.OutpointsAdapters.Resources.Resources", typeof(InfraOutpointsResourceService).Assembly);
        }

        public string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}
