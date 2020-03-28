using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Dapr.Actors.AspNetCore;

namespace AzureTf.BuilderActor {

    public class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseActors(actorRuntime => {
                    actorRuntime.RegisterActor<JobActor>();
                });
    }
}