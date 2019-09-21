using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace StellaWebInterface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StellaWebInterface stellaWebInterface = new StellaWebInterface(args);
            stellaWebInterface.Start();
        }
    }
}
