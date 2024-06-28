using DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using System.Runtime.CompilerServices;

namespace Business.DatabaseContext
{
    public class InMemoryDbContext : DbContext
    {
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowStep> FlowSteps { get; set; }
        public DbSet<Execution> Executions { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "FlowAutoClicker");
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.ApplyConfiguration(new FlowConfiguration());
            builder.ApplyConfiguration(new FlowStepConfiguration());
            builder.ApplyConfiguration(new ExecutionConfiguration());
        }
    }
}
