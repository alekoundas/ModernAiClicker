using Business.Helpers;
using Business.Configurations;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Business.DatabaseContext
{
    public class InMemoryDbContext : DbContext
    {
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowStep> FlowSteps { get; set; }
        public DbSet<FlowParameter> FlowParameters { get; set; }
        public DbSet<Execution> Executions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataSource = Path.Combine(PathHelper.GetDatabaseDataPath(), "StepinFlowDatabase.db");
            optionsBuilder.UseSqlite($"Data Source={dataSource};");

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.ApplyConfiguration(new FlowConfiguration());
            builder.ApplyConfiguration(new FlowStepConfiguration());
            builder.ApplyConfiguration(new ExecutionConfiguration());
            builder.ApplyConfiguration(new FlowParameterConfiguration());
        }
    }
}
