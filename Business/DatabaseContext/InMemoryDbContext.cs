using Business.Helpers;
using DataAccess.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using OpenCvSharp;
using System.Data.Common;
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
            var dataSource = Path.Combine(PathHelper.GetAppDataPath(), "AutoFlowClicker.db");
            optionsBuilder
                .UseSqlite($"Data Source={dataSource};");

            //optionsBuilder.UseSqlite("Data Source=AutoFlowClicker.db");

            //optionsBuilder.UseInMemoryDatabase(databaseName: "FlowAutoClicker");
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
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
