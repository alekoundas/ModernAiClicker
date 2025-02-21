using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model.Enums;
using Model.Models;
using StepinFlow.Views.Pages.Executions;
using StepinFlow.Views.Pages.FlowDetail;
using StepinFlow.Views.Pages.FlowStepDetail;
using System.ComponentModel;
using System.Windows;

namespace StepinFlow.ViewModels.UserControls
{
    public partial class FlowStepFrameUserControlViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IServiceProvider _serviceProvider;

        // FlowStep Type
        [ObservableProperty]
        private FlowStepTypesEnum _selectedType = FlowStepTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowStepTypesEnum> _Types = Enum.GetValues(typeof(FlowStepTypesEnum)).Cast<FlowStepTypesEnum>().ToList();
        [ObservableProperty]
        private bool _isTypeEnabled = false;
        [ObservableProperty]
        private Visibility _TypeVisibility = Visibility.Collapsed;


        // Flow Type
        [ObservableProperty]
        private FlowStepTypesEnum _selectedFlowType = FlowStepTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowStepTypesEnum> _flowTypes = Enum.GetValues(typeof(FlowStepTypesEnum)).Cast<FlowStepTypesEnum>().ToList();
        [ObservableProperty]
        private bool _isFlowTypeEnabled = false;
        [ObservableProperty]
        private Visibility _flowTypeVisibility = Visibility.Collapsed;


        [ObservableProperty]
        private IFlowStepDetailPage? _frameFlowStep;
        [ObservableProperty]
        private IFlowDetailPage? _frameFlow;
        [ObservableProperty]
        private IExecutionPage? _frameExecution;

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IFlowStepDetailPage>> _flowStepPageFactory;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>> _executionFlowStepPageFactory;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IFlowDetailPage>> _flowPageFactory;
        private FlowStep? _newFlowStep = null;

        public FlowStepFrameUserControlViewModel(IBaseDatawork baseDatawork, IServiceProvider serviceProvider)
        {
            _baseDatawork = baseDatawork;
            _serviceProvider = serviceProvider;

            // Lazy load the instances needed and not all at once.
            _flowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IFlowStepDetailPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchLoopFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchLoopFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WaitForTemplateFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorMoveFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorClickFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorScrollFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<SleepFlowStepPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IFlowDetailPage>>
            {
                { FlowStepTypesEnum.FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowStepTypesEnum.SUB_FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowStepTypesEnum.NO_SELECTION, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            };

            _executionFlowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchLoopExecutionPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WaitForTemplateExecutionPage>()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorMoveExecutionPage>()) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorClickExecutionPage>()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorScrollExecutionPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<SleepExecutionPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<GoToExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowResizeExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowMoveExecutionPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<LoopExecutionPage>()) }
            };

            //_executionFlowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IExecutionPage>>
            //{
            //    { FlowTypesEnum.FLOW, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<FlowExecutionPage>()) },
            //    { FlowTypesEnum.SUB_FLOW, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            //    { FlowTypesEnum.NO_SELECTION, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            //};

        }

        public void NavigateToNewFlowStep(FlowStep newFlowStep)
        {
            // Navigate to new flow step.
            IsTypeEnabled = true;
            TypeVisibility = Visibility.Visible;
            FlowTypeVisibility = Visibility.Collapsed;
            SelectedType = FlowStepTypesEnum.NO_SELECTION;
            _newFlowStep = newFlowStep;
            FrameFlowStep = null;
            FrameExecution = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            // Navigate to existing flow step.
            FlowStepTypesEnum? Type = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();

            if (Type != null)
            {
                SelectedType = Type.Value;
                TypeVisibility = Visibility.Visible;
                FlowTypeVisibility = Visibility.Collapsed;
                IsTypeEnabled = false;
                NavigateToFlowStepDetailPage(id);
            }

        }
        public async Task NavigateToFlow(int id)
        {
            // Navigate to existing flow.
            FlowStepTypesEnum? flowType = await _baseDatawork.Flows.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();

            if (flowType != null)
            {
                SelectedFlowType = flowType.Value;
                IsFlowTypeEnabled = false;
                TypeVisibility = Visibility.Collapsed;
                FlowTypeVisibility = Visibility.Visible;
                NavigateToFlowDetailPage(id);
            }

        }

        public void NavigateToExecution(Execution execution)
        {
            if (execution.FlowStep != null)
            {
                SelectedType = execution.FlowStep.Type;
                TypeVisibility = Visibility.Visible;
                FlowTypeVisibility = Visibility.Collapsed;
                IsTypeEnabled = false;
                IsFlowTypeEnabled = false;
                NavigateToExecutionDetailPage(execution);

            }
            else if (execution.Flow != null)
            {
                SelectedFlowType = execution.Flow.Type;
                TypeVisibility = Visibility.Collapsed;
                FlowTypeVisibility = Visibility.Visible;
                IsTypeEnabled = false;
                IsFlowTypeEnabled = false;
                //NavigateToExecutionDetailPage(execution);
            }
        }

       



        [RelayCommand]
        private void OnTypeSelectionChanged()
        {
            if (_newFlowStep != null)
            {
                FrameFlow = null;
                _newFlowStep.Type = SelectedType;
                NavigateToNewFlowStepDetailPage(_newFlowStep);
            }
        }

        [RelayCommand]
        private void OnFlowTypeSelectionChanged()
        {
        }


        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            FrameFlow = null;
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadNewFlowStep(newFlowStep);
                FrameFlowStep = page;
            }
            else
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameExecution = null;
            }
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            FrameFlow = null;
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FrameFlowStep = page;
            }
            else
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameExecution = null;
            }
        }

        private void NavigateToFlowDetailPage(int id)
        {
            FrameFlowStep = null;
            IFlowDetailPage? page = _flowPageFactory.TryGetValue(SelectedFlowType, out Lazy<IFlowDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowId(id);
                FrameFlow = page;
            }
            else
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameExecution = null;
            }
        }

        private void NavigateToExecutionDetailPage(Execution execution)
        {
            FrameFlowStep = null;
            IExecutionPage? page = _executionFlowStepPageFactory.TryGetValue(SelectedType, out Lazy<IExecutionPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.SetExecution(execution);
                FrameExecution = page;
            }
            else
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameExecution = null;
            }
        }

    }
}