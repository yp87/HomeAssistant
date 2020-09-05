using System.IO;
using System.Text.Json;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supervisor.ActionHandlers;
using Supervisor.Automation;
using Supervisor.FilesUpdater;

namespace Supervisor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var secret = File.ReadAllText("secret_webhook");
            builder.RegisterInstance(secret);

            builder.RegisterType<CheckRunActionHandler>()
                .As<IActionHandler>()
                // Because of the semaphore in the implementation.
                .SingleInstance();

            builder.RegisterType<AutomationUpdater>()
                .As<IAutomationUpdater>();

            builder.RegisterType<GitAutomationRepository>()
                .As<ISourceController>();

            builder.RegisterType<SourceControlFilesUpdater>()
                .As<IFilesUpdater>();

            builder.RegisterType<HomeAssistantNotifier>()
                .As<IAutomationNotifier>();

            builder.RegisterType<DockerComposeHomeAssistantDeployer>()
                .As<IAutomationDeployer>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
