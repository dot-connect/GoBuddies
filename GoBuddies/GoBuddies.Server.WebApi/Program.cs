using GoBuddies.Server.WebApi;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoBuddies.Server.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                          .UseKestrel()
                          .UseStartup<Startup>()
                          .Build();

            host.Run();
        }
    }
}
