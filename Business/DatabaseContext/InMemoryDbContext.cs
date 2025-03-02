using Business.Helpers;
using Business.Configurations;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.Enums;
using System.Windows;

namespace Business.DatabaseContext
{
    public class InMemoryDbContext : DbContext
    {
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowStep> FlowSteps { get; set; }
        public DbSet<FlowParameter> FlowParameters { get; set; }
        public DbSet<Execution> Executions { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dataPath = PathHelper.GetDatabaseDataPath();
            string dataSource = Path.Combine(dataPath, "StepinFlowDatabase.db");

            // Create the directory if it doesn’t exist
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath); 

            // Create the database file if it doesn’t exist.
            if (!File.Exists(dataSource))
                File.Create(dataSource).Close(); 

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
            builder.ApplyConfiguration(new AppSettingConfiguration());
            builder.ApplyConfiguration(new FlowParameterConfiguration());
        }

        public void RunMigrations()
        {
            this.Database.Migrate();
        }


        public void TrySeedInitialData()
        {
            if (!AppSettings.Any())
            {
                // Main window location and size.
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.MAIN_WINDOW_LEFT, Value = ((SystemParameters.PrimaryScreenWidth - 1000) / 2).ToString() });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.MAIN_WINDOW_TOP, Value = ((SystemParameters.PrimaryScreenHeight - 600) / 2).ToString() });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.MAIN_WINDOW_WIDTH, Value = "1000" });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.MAIN_WINDOW_HEIGHT, Value = "600" });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.IS_MAIN_WINDOW_MAXIMAZED, Value = "false" });

                // Selector window location and size.
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.SELECTOR_WINDOW_LEFT, Value = ((SystemParameters.PrimaryScreenWidth - 1000) / 2).ToString() });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.SELECTOR_WINDOW_TOP, Value = ((SystemParameters.PrimaryScreenHeight - 600) / 2).ToString() });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.SELECTOR_WINDOW_WIDTH, Value = "1000" });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.SELECTOR_WINDOW_HEIGHT, Value = "600" });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.IS_SELECTOR_WINDOW_MAXIMAZED, Value = "false" });

                // Execution history log settings.
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.EXECUTION_HISTORY_LOG_ACCURACY, Value = "80" });
                AppSettings.Add(new AppSetting { Key = AppSettingsEnum.IS_EXECUTION_HISTORY_LOG_ENABLED, Value = "true" });

                SaveChanges();
            }
        }
    }
}
