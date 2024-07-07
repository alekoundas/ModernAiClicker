using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    
    public partial class CursorMoveExecutionPage : Page
    {

        public CursorMoveExecutionViewModel ViewModel { get; }

        public CursorMoveExecutionPage(CursorMoveExecutionViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            DataContext = this;

        }

    }
}
