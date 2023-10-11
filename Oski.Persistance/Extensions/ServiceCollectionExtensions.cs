using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oski.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Oski.Application.Interfaces.Repositories;
using Oski.Persistance.Repository;
using Microsoft.Extensions.Configuration;
using Oski.Application.Interfaces;
using Oski.Application.Extensions;

namespace Oski.Persistance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddRepositories();
        }

 

        public static void AddDbContext(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDataContext>(options =>
               options.UseSqlServer(connectionString,
                   builder => builder.MigrationsAssembly(typeof(AppDataContext).Assembly.FullName)));
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient(typeof(IUnitOfWork),typeof(UnitOfWork))
                .AddTransient(typeof(IGenericRepository<>),typeof(GenericRepository<>))
                .AddTransient<IUserRepository,UserRepository>()
                .AddTransient<ITestService,TestService>()
                .AddTransient<ITestRepository,TestRepository>();
        }

    }
}
