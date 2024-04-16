// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AutoMapper;
using Business.AutoMapper;
using Business.DatabaseContext;
using Business.Interfaces;
using Business.Repository.Entities;
using Business.Repository.Interfaces;
using Business.Services;
using DataAccess;
using DataAccess.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Model.Models;
using ModernAiClicker.Services;
using ModernAiClicker.ViewModels;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Windows;
using ModernAiClicker.Views.Pages;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using ModernAiClicker.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Contracts;
using Wpf.Ui.Services;

namespace ModernAiClicker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                //services.addj()
                //       .AddJsonOptions(
                //           options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                //       );

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<BaseViewModel>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<IContentDialogService, ContentDialogService>();
                services.AddSingleton<ISystemService, SystemService>();
                services.AddSingleton<ITemplateMatchingService, TemplateMatchingService>();
                services.AddSingleton<IFlowService, FlowService>();

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();

                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();

                services.AddSingleton<FlowsPage>();
                services.AddSingleton<FlowsViewModel>();

                services.AddSingleton<ExecutionPage>();
                services.AddSingleton<ExecutionViewModel>();


                services.AddSingleton<FlowStepDetailNewSelectTypePage>();
                services.AddSingleton<FlowStepDetailNewSelectTypeViewModel>();

                services.AddSingleton<FrameExecutionFlowPage>();
                services.AddSingleton<FrameExecutionFlowViewModel>();

                services.AddSingleton<FlowStepDetailTemplateMatchingPage>();
                services.AddSingleton<FlowStepDetailTemplateMatchingViewModel>();

                services.AddSingleton<FlowStepDetailMouseClickPage>();
                services.AddSingleton<FlowStepDetailMouseClickViewModel>();

                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

                //services.AddAutoMapper(x => new MapperConfig());

                services.AddDbContext<InMemoryDbContext>();
                //options =>
                //{
                //    options..UseSqlite("Data Source=customers.db");
                //    options.UseLazyLoadingProxies();
                //});


                var dbContext = services.BuildServiceProvider().GetService<InMemoryDbContext>();
                if (dbContext != null)
                {
                    //var steps = new ObservableCollection<FlowStep>();
                    //steps.Add(new FlowStep() { IsNew = true });

                    //dbContext.Flows.Add(new Flow()
                    //{
                    //    Name = "",
                    //    Favorite = true,
                    //    IsNew = true,
                    //    FlowSteps = steps
                    //});
                    var ss = SetTestData();
                    if (ss != null)
                    {

                        dbContext.Flows.AddRange(ss);
                        dbContext.SaveChanges();
                    }
                }

                services.AddScoped<IBaseDatawork, BaseDatawork>();

                services.AddScoped<IFlowRepository, FlowRepository>();
                services.AddScoped<IFlowStepRepository, FlowStepRepository>();



                //MapperConfig.InitializeAutomapper();

            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            _host.Start();
            //v_host.Services.GetRequiredService<>()
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }





        private static ObservableCollection<Flow>? SetTestData()
        {
            var systemService = new SystemService();

            List<Flow>? flows = systemService.LoadFlowsJSON();

            if (flows == null)
                return null;

            return new ObservableCollection<Flow>(flows);
        }


    }
}
