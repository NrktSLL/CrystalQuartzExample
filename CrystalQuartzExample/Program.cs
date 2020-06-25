using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CrystalQuartzExample
{
    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            static void Configure(IWebHostBuilder webBuilder) => webBuilder.UseStartup<Startup>();

            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(Configure);
        }
    }
}
