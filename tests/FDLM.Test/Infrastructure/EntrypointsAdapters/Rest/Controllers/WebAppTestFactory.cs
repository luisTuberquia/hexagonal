using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Test.Infrastructure.EntrypointsAdapters.Rest.Controllers
{
    public class WebAppTestFactory : WebApplicationFactory<Program>
    {        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Opcional: Personaliza la configuración del host de prueba aquí
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("local");
            return base.CreateHost(builder);
        }
    }
}
