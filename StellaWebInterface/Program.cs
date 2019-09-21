using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using StellaWebInterface.GlobalObjects;

namespace StellaWebInterface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context context = Context.Instance;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
       {
           return WebHost
               .CreateDefaultBuilder(args)
               .UseStartup<Startup>();
       }

        public void SetConnectedRaspberries(int number)
       {
           
       }
    }
}
