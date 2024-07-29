using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Resources
{
    public class DomainResourceService : IDomainResourceService
    {
        private readonly ResourceManager _resourceManager;

        public DomainResourceService()
        {            
            _resourceManager = new ResourceManager("FDLM.Domain.Resources.Resources", typeof(DomainResourceService).Assembly);
        }

        public string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}
