using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
        }

        private IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}