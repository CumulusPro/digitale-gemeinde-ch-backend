using Cpro.Forms.Service.Interface;
using Cpro.Forms.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Cpro.Forms.Service.Infrastructure
{
    public class ServiceModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddTransient<IFormDesignerHistoryService, FormDesignerHistoryService>();
            services.AddTransient<IFormDesignerService, FormDesignerService>();
            services.AddTransient<IFormService, FormService>();
            services.AddTransient<IFormTemplateService, FormTemplateService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ITenantDesignService, TenantDesignService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}
