using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.TimedTasks.Configuration
{
    public static class Configurator
    {
        internal static IServiceProvider serviceProvider { get; set; }

        public static void ConfigureTimeTasks(this IServiceCollection services)
        {
            services.AddSingleton<TaskServices>();

        }

        public static void InitTimeTasks(this IApplicationBuilder app)
        {
            serviceProvider = app.ApplicationServices;



            Func<Type, ITask> createTaskFromType = (Type type) =>
            {
                return app.ApplicationServices.GetService(type) as ITask;
            };

            foreach (var t in Core.GetTasks(createTaskFromType))
            {
                Core.AddTask(t);
            }
        }
    }
}
