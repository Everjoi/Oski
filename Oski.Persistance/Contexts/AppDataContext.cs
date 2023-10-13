using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Oski.Domain.Common;
using Oski.Domain.Common.Interfaces;
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace Oski.Persistance.Contexts
{
    public class AppDataContext : DbContext
    {
        private readonly string _connectionString;
        private readonly IHostingEnvironment _environment;

        public AppDataContext( DbContextOptions<AppDataContext> options, IConfiguration configuration,IHostingEnvironment environment)
            : base(options)
        {
            _connectionString = GetConnectionString(configuration);
            _environment = environment;
        }

        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options) { }

        public AppDataContext() : base() { }


        public static string GetConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection");
        }


        public DbSet<User> Users => Set<User>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<UserTestAttempt> TestAttempts => Set<UserTestAttempt>();
        public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


             
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId);

            
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);

            
            modelBuilder.Entity<UserTestAttempt>()
                .HasOne(uta => uta.User)
                .WithMany()
                .HasForeignKey(uta => uta.UserId);

            
            modelBuilder.Entity<UserTestAttempt>()
                .HasOne(uta => uta.Test)
                .WithMany()
                .HasForeignKey(uta => uta.TestId);


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseInMemoryDatabase("OskiDb");  // For Tests
            optionsBuilder.UseSqlServer(_connectionString);   // For Development
        }




        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
             
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }


    }
}
