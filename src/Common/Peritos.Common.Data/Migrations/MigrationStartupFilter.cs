using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Peritos.Common.Data.Migrations
{
    //A startup filter that when a web app starts, it migrates the context. 
    public class MigrationStartupFilter<TContext> : IStartupFilter where TContext : DbContext
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<TContext>().Database.SetCommandTimeout(160);
                    scope.ServiceProvider.GetRequiredService<TContext>().Database.Migrate();
                }
                next(app);
            };
        }
    }
}
