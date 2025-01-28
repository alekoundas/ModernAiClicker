// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Business.DatabaseContext;
using Business.Factories;
using Business.Factories.Workers;
using Business.Helpers;
using Business.Interfaces;
using Business.Repository.Entities;
using Business.Repository.Interfaces;
using Business.Services;
using DataAccess;
using DataAccess.Repository;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Model.Models;
using ModernAiClicker.Services;
using ModernAiClicker.ViewModels;
using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using ModernAiClicker.ViewModels.UserControls;
using ModernAiClicker.ViewModels.Windows;
using ModernAiClicker.Views.Pages;
using ModernAiClicker.Views.Pages.Executions;
using ModernAiClicker.Views.Pages.FlowStepDetail;
using ModernAiClicker.Views.UserControls;
using ModernAiClicker.Views.Windows;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
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
        private static readonly string? _basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => c.SetBasePath(_basePath ?? ""))
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                // Repository
                //services.AddScoped<IDbContextFactory, DbContextFactory>();
                services.AddTransient<IBaseDatawork, BaseDatawork>();
                services.AddScoped<IFlowRepository, FlowRepository>();
                services.AddScoped<IFlowStepRepository, FlowStepRepository>();
                services.AddScoped<IExecutionRepository, ExecutionRepository>();


                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<IContentDialogService, ContentDialogService>();
                services.AddScoped<ISystemService, SystemService>();
                services.AddScoped<ITemplateSearchService, TemplateSearchService>();

                services.AddScoped<IExecutionFactory, ExecutionFactory>();
                services.AddScoped<WindowMoveExecutionWorker>();
                services.AddScoped<WindowResizeExecutionWorker>();
                services.AddScoped<MouseMoveExecutionWorker>();
                services.AddScoped<MouseClickExecutionWorker>();
                services.AddScoped<MouseScrollExecutionWorker>();
                services.AddScoped<TemplateSearchExecutionWorker>();
                services.AddScoped<TemplateSearchLoopExecutionWorker>();
                services.AddScoped<MultipleTemplateSearchExecutionWorker>();
                services.AddScoped<MultipleTemplateSearchLoopExecutionWorker>();
                services.AddScoped<SleepExecutionWorker>();
                services.AddScoped<GoToExecutionWorker>();

                // User Controls
                services.AddSingleton<TreeViewUserControl>();
                services.AddSingleton<TreeViewUserControlViewModel>();

                // Pages
                //Tabs
                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();

                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();

                services.AddSingleton<FlowsPage>();
                services.AddSingleton<FlowsViewModel>();

                services.AddSingleton<ExecutionPage>();
                services.AddSingleton<ExecutionViewModel>();

                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

                //Flow step detail
                services.AddSingleton<NewSelectTypeFlowStepPage>();
                services.AddSingleton<NewSelectTypeFlowStepViewModel>();

                services.AddSingleton<TemplateSearchFlowStepPage>();
                services.AddSingleton<TemplateSearchFlowStepViewModel>();

                services.AddSingleton<TemplateSearchLoopFlowStepPage>();
                services.AddSingleton<TemplateSearchLoopFlowStepViewModel>();

                services.AddSingleton<MultipleTemplateSearchLoopFlowStepPage>();
                services.AddSingleton<MultipleTemplateSearchLoopFlowStepViewModel>();

                services.AddSingleton<MultipleTemplateSearchFlowStepPage>();
                services.AddSingleton<MultipleTemplateSearchFlowStepViewModel>();

                services.AddSingleton<CursorClickFlowStepPage>();
                services.AddSingleton<CursorClickFlowStepViewModel>();

                services.AddSingleton<CursorMoveFlowStepPage>();
                services.AddSingleton<CursorMoveFlowStepViewModel>();

                services.AddSingleton<SleepFlowStepPage>();
                services.AddSingleton<SleepFlowStepViewModel>();

                services.AddSingleton<GoToFlowStepPage>();
                services.AddSingleton<GoToFlowStepViewModel>();

                services.AddSingleton<WindowResizeFlowStepViewModel>();
                services.AddSingleton<WindowResizeFlowStepPage>();

                services.AddSingleton<WindowMoveFlowStepViewModel>();
                services.AddSingleton<WindowMoveFlowStepPage>();

                //Flow execution step detail

                services.AddSingleton<TemplateSearchExecutionPage>();
                services.AddSingleton<TemplateSearchExecutionViewModel>();

                services.AddSingleton<TemplateSearchLoopExecutionPage>();
                services.AddSingleton<TemplateSearchLoopExecutionViewModel>();

                services.AddSingleton<MultipleTemplateSearchLoopExecutionPage>();
                services.AddSingleton<MultipleTemplateSearchLoopExecutionViewModel>();

                services.AddSingleton<MultipleTemplateSearchExecutionPage>();
                services.AddSingleton<MultipleTemplateSearchExecutionViewModel>();

                services.AddSingleton<CursorClickExecutionPage>();
                services.AddSingleton<CursorClickExecutionViewModel>();

                services.AddSingleton<CursorMoveExecutionPage>();
                services.AddSingleton<CursorMoveExecutionViewModel>();

                services.AddSingleton<CursorScrollExecutionPage>();
                services.AddSingleton<CursorScrollExecutionViewModel>();

                services.AddSingleton<SleepExecutionPage>();
                services.AddSingleton<SleepExecutionViewModel>();

                services.AddSingleton<GoToExecutionPage>();
                services.AddSingleton<GoToExecutionViewModel>();

                services.AddSingleton<WindowResizeExecutionViewModel>();
                services.AddSingleton<WindowResizeExecutionPage>();

                services.AddSingleton<WindowMoveExecutionViewModel>();
                services.AddSingleton<WindowMoveExecutionPage>();



                // DB context
                services.AddDbContext<InMemoryDbContext>(ServiceLifetime.Transient);
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
            //DispatcherUnhandledException += app_DispatcherUnhandledException;
            _host.Start();
            //v_host.Services.GetRequiredService<>()
        }
        //static void app_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    throw e.Exception;
        //    // Log/inspect the inspection here
        //}
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





        private static ObservableCollection<Flow>? GetFlowsFromJson()
        {
            var systemService = new SystemService();

            List<Flow>? flows = systemService.LoadFlowsJSON();

            if (flows == null)
                return null;

            return new ObservableCollection<Flow>(flows);
        }
    }
}
