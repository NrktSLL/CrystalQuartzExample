using System;
using System.Collections.Specialized;
using CrystalQuartz.AspNetCore;
using CrystalQuartzExample.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace CrystalQuartzExample
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            static void ConfigureOptions(KestrelServerOptions options) => options.AllowSynchronousIO = true;

            services.Configure<KestrelServerOptions>(ConfigureOptions);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            if (InitializeJobsAsync() != null) app.UseCrystalQuartz(InitializeJobsAsync);

            app.UseEndpoints(Action);
        }

        private static void Action(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/", async context => await context.Response.WriteAsync("Task Started"));

        private IScheduler InitializeJobsAsync()
        {
            try
            {
                var properties = new NameValueCollection { ["quartz.scheduler.instanceName"] = "Quartz Example" };
                var schedulerFactory = new StdSchedulerFactory(properties);
                var scheduler = schedulerFactory.GetScheduler().Result;

                #region Jobs

                var job1 = JobBuilder.Create<Job1>()
                    .WithIdentity("testJob1", "Test")
                    .WithDescription("explanation example")
                    .Build();

                var job1Trigger = TriggerBuilder.Create()
                    .WithIdentity("testJob1Trigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(2).RepeatForever())
                    .Build();

                var job2 = JobBuilder.Create<Job2>()
                    .WithIdentity("testJob2", "Test")
                    .WithDescription("explanation example")
                    .Build();

                var job2Trigger = TriggerBuilder.Create()
                    .WithIdentity("testJob1Trigger2")
                    .StartNow()
                    .WithCronSchedule("0 0/1 * 1/1 * ? *") //1 Min
                    .Build();

                var job3 = JobBuilder.Create<Job3>()
                    .WithIdentity("testJob3", "Another Group")
                    .WithDescription("explanation example")
                    .Build();

                var job3Trigger = TriggerBuilder.Create()
                    .WithIdentity("testJob1Trigger3")
                    .StartNow()
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                    .Build();

                scheduler.ScheduleJob(job1, job1Trigger);
                scheduler.ScheduleJob(job2, job2Trigger);
                scheduler.ScheduleJob(job3, job3Trigger);

                #endregion

                scheduler.Start();

                return scheduler;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
