using System;
using Core.Interfaces.Common;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;
using Service;
using Service.Base;
using WebApi.Service;

namespace WebApi.Extensions
{
    public static class ServiceRegister
    {
        public static void RegisterDependency(this IServiceCollection services)
        {
            #region Services

            services.AddScoped(typeof(IBaseService<,>),typeof(BaseService<,>));

            #endregion

            #region Repositories

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            #endregion

            #region Others

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IFileUploadService, FileUploadService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IWorkerService, WorkerService>();

            #endregion
        }
    }
}
