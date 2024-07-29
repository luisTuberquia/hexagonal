using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Resources
{
    public interface IDomainResourceService
    {
        string GetString(string name);
    }
}
