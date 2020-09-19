using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using StellaWebInterface.GlobalObjects;

namespace StellaWebInterface
{
    public class StellaWebInterface
    {
        private readonly string[] _args;
        private readonly Context _context;

        public StellaWebInterface(string[] args)
        {
            _args = args;
            _context = Context.Instance;
        }

        public void Start()
        {
            CreateWebHostBuilder(_args).Build().Run();
//            BuildWebHost(_args).Run();
        }

        private IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
        
        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            var enviroment = config["environment"] ?? "Development";

            return WebHost.CreateDefaultBuilder(args)
                .UseEnvironment(enviroment)
                .UseStartup<Startup>()
                .Build();
        }
    }
}