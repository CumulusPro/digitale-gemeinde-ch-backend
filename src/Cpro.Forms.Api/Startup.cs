using Microsoft.AspNetCore.Mvc;
using Peritos.Common.Api.Controllers;
using Peritos.Common.Api.Cors;
using Peritos.Common.Api.Swagger;
using Peritos.Common.DependencyInjection;
using Peritos.Common.Api.Authentication;
using Peritos.Common.Abstractions;
using Cpro.Forms.Api.Infrastructure.Middleware;
using Cpro.Forms.Api.Infrastructure.Security;

[assembly: ApiController]
namespace Cpro.Forms.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        services.AddPKSControllers(Configuration);
        services.AddPKSAuthentication(Configuration);
        services.AddPKSSwagger(Configuration);
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
        services.AddScoped<IRequestContext, ApiRequestContext>();
        // services.AddScoped<ApiKeyAuthFilter>();
        services.AddModules();

        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseHsts();
        app.UsePKSCors(Configuration);
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UsePKSAuthentication(Configuration);
        app.UseApiRequestContext();
        app.UsePKSSwagger(Configuration);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
