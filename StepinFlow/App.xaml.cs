// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Business.DatabaseContext;
using Business.Factories;
using Business.Factories.Workers;
using Business.Interfaces;
using Business.Repository;
using Business.Repository.Entities;
using Business.Repository.Interfaces;
using Business.Services;
using DataAccess.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StepinFlow.Interfaces;
using StepinFlow.Services;
using StepinFlow.ViewModels.Pages;
using StepinFlow.ViewModels.Pages.Executions;
using StepinFlow.ViewModels.UserControls;
using StepinFlow.ViewModels.Windows;
using StepinFlow.Views.Pages;
using StepinFlow.Views.Pages.Executions;
using StepinFlow.Views.Pages.FlowDetail;
using StepinFlow.Views.Pages.FlowStepDetail;
using StepinFlow.Views.UserControls;
using StepinFlow.Views.Windows;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;

namespace StepinFlow
{
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

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Windows.
                services.AddTransient<ScreenshotSelectionWindow>();
                services.AddTransient<ScreenshotSelectionWindowViewModel>();

                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                // Services.
                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IContentDialogService, ContentDialogService>();

                services.AddScoped<ISystemService, SystemService>();
                services.AddScoped<IWindowService, WindowService>();
                services.AddScoped<ITemplateSearchService, TemplateSearchService>();
                services.AddTransient<IBaseDatawork, BaseDatawork>();

                // Repository
                services.AddScoped<IFlowRepository, FlowRepository>();
                services.AddScoped<IFlowStepRepository, FlowStepRepository>();
                services.AddScoped<IExecutionRepository, ExecutionRepository>();

                // DB context
                services.AddDbContext<InMemoryDbContext>(ServiceLifetime.Transient);

                // Factory.
                services.AddScoped<IExecutionFactory, ExecutionFactory>();
                services.AddScoped<GoToExecutionWorker>();
                services.AddScoped<SleepExecutionWorker>();
                services.AddScoped<MouseMoveExecutionWorker>();
                services.AddScoped<WindowMoveExecutionWorker>();
                services.AddScoped<MouseClickExecutionWorker>();
                services.AddScoped<MouseScrollExecutionWorker>();
                services.AddScoped<WindowResizeExecutionWorker>();
                services.AddScoped<TemplateSearchExecutionWorker>();
                services.AddScoped<TemplateSearchLoopExecutionWorker>();
                services.AddScoped<MultipleTemplateSearchExecutionWorker>();
                services.AddScoped<MultipleTemplateSearchLoopExecutionWorker>();

                // User Controls
                services.AddSingleton<TreeViewUserControl>();
                services.AddSingleton<TreeViewUserControlViewModel>();

                services.AddSingleton<FlowStepFrameUserControl>();
                services.AddSingleton<FlowStepFrameUserControlViewModel>();

                // Pages
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

                // Flow detail.
                services.AddSingleton<FlowPage>();
                services.AddSingleton<FlowViewModel>();

                // Flow step detail.
                services.AddSingleton<TemplateSearchFlowStepPage>();
                services.AddSingleton<TemplateSearchFlowStepViewModel>();

                services.AddSingleton<TemplateSearchLoopFlowStepPage>();
                services.AddSingleton<TemplateSearchLoopFlowStepViewModel>();

                services.AddSingleton<MultipleTemplateSearchLoopFlowStepPage>();
                services.AddSingleton<MultipleTemplateSearchLoopFlowStepViewModel>();

                services.AddSingleton<MultipleTemplateSearchFlowStepPage>();
                services.AddSingleton<MultipleTemplateSearchFlowStepViewModel>();

                services.AddSingleton<WaitForTemplateFlowStepPage>();
                services.AddSingleton<WaitForTemplateFlowStepViewModel>();

                services.AddSingleton<CursorClickFlowStepPage>();
                services.AddSingleton<CursorClickFlowStepViewModel>();

                services.AddSingleton<CursorMoveFlowStepPage>();
                services.AddSingleton<CursorMoveFlowStepViewModel>();

                services.AddSingleton<CursorScrollFlowStepPage>();
                services.AddSingleton<CursorScrollFlowStepViewModel>();

                services.AddSingleton<SleepFlowStepPage>();
                services.AddSingleton<SleepFlowStepViewModel>();

                services.AddSingleton<GoToFlowStepPage>();
                services.AddSingleton<GoToFlowStepViewModel>();

                services.AddSingleton<WindowResizeFlowStepPage>();
                services.AddSingleton<WindowResizeFlowStepViewModel>();

                services.AddSingleton<WindowMoveFlowStepPage>();
                services.AddSingleton<WindowMoveFlowStepViewModel>();

                services.AddSingleton<LoopFlowStepPage>();
                services.AddSingleton<LoopFlowStepViewModel>();

                //Flow execution step detail
                services.AddSingleton<TemplateSearchExecutionPage>();
                services.AddSingleton<TemplateSearchExecutionViewModel>();

                services.AddSingleton<TemplateSearchLoopExecutionPage>();
                services.AddSingleton<TemplateSearchLoopExecutionViewModel>();

                services.AddSingleton<MultipleTemplateSearchLoopExecutionPage>();
                services.AddSingleton<MultipleTemplateSearchLoopExecutionViewModel>();

                services.AddSingleton<MultipleTemplateSearchExecutionPage>();
                services.AddSingleton<MultipleTemplateSearchExecutionViewModel>();

                services.AddSingleton<WaitForTemplateExecutionPage>();
                services.AddSingleton<WaitForTemplateExecutionViewModel>();

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

                services.AddSingleton<LoopExecutionViewModel>();
                services.AddSingleton<LoopExecutionPage>();

            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T? GetService<T>()
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
    }
}
