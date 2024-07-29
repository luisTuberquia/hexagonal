using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Application.Resources
{
    internal class ApplicationResourceService : IApplicationResourceService
    {
        private readonly ResourceManager _resourceManager;

        public ApplicationResourceService()
        {            
            _resourceManager = new ResourceManager("FDLM.Application.Resources.Resources", typeof(ApplicationResourceService).Assembly);
        }

        public string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}
