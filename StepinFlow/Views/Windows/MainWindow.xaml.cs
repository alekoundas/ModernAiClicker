using Microsoft.Extensions.DependencyInjection;
using StepinFlow.Services;
using StepinFlow.ViewModels.Windows;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowVM ViewModel { get; }

        public MainWindow(
            MainWindowVM viewModel,
            INavigationViewPageProvider pageService,
            INavigationService navigationService,
            IServiceProvider serviceProvider
        )
        {
            ViewModel = viewModel;
            DataContext = this;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            var pageProvider = serviceProvider.GetRequiredService<INavigationViewPageProvider>();

            // NavigationControl is x:Name of our NavigationView defined in XAML.
            RootNavigation.SetPageProviderService(pageProvider);
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
        }


        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        //public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();


        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }


        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            //RootNavigation.SetPageProviderService(serviceProvider);
            throw new NotImplementedException();
        }

        public void SetPageService(INavigationViewPageProvider navigationViewPageProvider)
        {
            RootNavigation.SetPageProviderService(navigationViewPageProvider);
        }
    }
}
