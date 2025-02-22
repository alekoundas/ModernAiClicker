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
        private FlowStepTypesEnum _selectedFlowStepType = FlowStepTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowStepTypesEnum> _flowStepTypes = Enum.GetValues(typeof(FlowStepTypesEnum)).Cast<FlowStepTypesEnum>().ToList();
        [ObservableProperty]
        private Visibility _flowStepVisibility = Visibility.Collapsed;

        // FlowParameter Type
        [ObservableProperty]
        private FlowParameterTypesEnum _selectedFlowParameterType = FlowParameterTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowParameterTypesEnum> _flowParameterTypes = Enum.GetValues(typeof(FlowParameterTypesEnum)).Cast<FlowParameterTypesEnum>().ToList();
        [ObservableProperty]
        private Visibility _flowParameterVisibility = Visibility.Collapsed;


        // Flow Type
        [ObservableProperty]
        private FlowStepTypesEnum _selectedFlowType = FlowStepTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowStepTypesEnum> _flowTypes = Enum.GetValues(typeof(FlowStepTypesEnum)).Cast<FlowStepTypesEnum>().ToList();
        [ObservableProperty]
        private Visibility _flowVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private bool _isEnabled = false;

        [ObservableProperty]
        private IFlowStepDetailPage? _frameFlowStep;
        [ObservableProperty]
        private IFlowParameterDetailPage? _frameFlowParameter;
        [ObservableProperty]
        private IFlowDetailPage? _frameFlow;
        [ObservableProperty]
        private IExecutionPage? _frameExecution;

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IFlowStepDetailPage>> _flowStepPageFactory;
        private readonly Dictionary<FlowParameterTypesEnum, Lazy<IFlowParameterDetailPage>> _flowParameterPageFactory;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>> _executionFlowStepPageFactory;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IFlowDetailPage>> _flowPageFactory;
        private FlowStep? _newFlowStep = null;
        private FlowParameter? _newFlowParameter = null;

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

            _flowParameterPageFactory = new Dictionary<FlowParameterTypesEnum, Lazy<IFlowParameterDetailPage>>
            {
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
            IsEnabled = true;
            FlowStepVisibility = Visibility.Visible;
            FlowVisibility = Visibility.Collapsed;
            FlowParameterVisibility = Visibility.Collapsed;
            SelectedFlowStepType = FlowStepTypesEnum.NO_SELECTION;
            _newFlowStep = newFlowStep;
            _newFlowParameter = null;
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
        }

        public void NavigateToNewFlowParameter(FlowParameter newFlowParameter)
        {
            // Navigate to new flow step.
            IsEnabled = true;
            FlowStepVisibility = Visibility.Collapsed;
            FlowVisibility = Visibility.Collapsed;
            FlowParameterVisibility = Visibility.Visible;

            SelectedFlowParameterType = FlowParameterTypesEnum.NO_SELECTION;
            _newFlowStep = null;
            _newFlowParameter = newFlowParameter;
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            // Navigate to existing flow step.
            FlowStepTypesEnum? Type = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();

            if (Type != null)
            {
                IsEnabled = false;
                SelectedFlowStepType = Type.Value;
                FlowStepVisibility = Visibility.Visible;
                FlowVisibility = Visibility.Collapsed;
                FlowParameterVisibility = Visibility.Collapsed;
                NavigateToFlowStepDetailPage(id);
            }

        }

        public async Task NavigateToFlowParameter(int id)
        {
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            // Navigate to existing flow step.
            FlowParameterTypesEnum? type = await _baseDatawork.FlowParameters.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();

            if (type != null)
            {
                IsEnabled = false;
                SelectedFlowParameterType = type.Value;
                FlowStepVisibility = Visibility.Collapsed;
                FlowParameterVisibility = Visibility.Visible;
                FlowVisibility = Visibility.Collapsed;
                NavigateToFlowStepDetailPage(id);
            }

        }
        public async Task NavigateToFlow(int id)
        {
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            // Navigate to existing flow.
            FlowStepTypesEnum? flowType = await _baseDatawork.Flows.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.Type)
                .FirstOrDefaultAsync();

            if (flowType != null)
            {
                IsEnabled = false;
                SelectedFlowType = flowType.Value;
                FlowStepVisibility = Visibility.Collapsed;
                FlowParameterVisibility = Visibility.Collapsed;
                FlowVisibility = Visibility.Visible;
                NavigateToFlowDetailPage(id);
            }

        }

        public void NavigateToExecution(Execution execution)
        {
            FrameFlow = null;
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            if (execution.FlowStep != null)
            {
                SelectedFlowType = execution.FlowStep.Type;
                FlowStepVisibility = Visibility.Visible;
                FlowVisibility = Visibility.Collapsed;
                FlowParameterVisibility = Visibility.Collapsed;
                IsEnabled = false;
                NavigateToExecutionDetailPage(execution);

            }
            else if (execution.Flow != null)
            {
                SelectedFlowType = execution.Flow.Type;
                FlowStepVisibility = Visibility.Collapsed;
                FlowVisibility = Visibility.Visible;
                FlowParameterVisibility = Visibility.Collapsed;
                IsEnabled = false;
                //NavigateToExecutionDetailPage(execution);
            }
        }





        [RelayCommand]
        private void OnTypeSelectionChanged()
        {
            if (_newFlowStep != null)
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameFlowParameter = null;
                FrameExecution= null;
                _newFlowStep.Type = SelectedFlowStepType;
                NavigateToNewFlowStepDetailPage(_newFlowStep);
            }
            else if (_newFlowParameter != null)
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameFlowParameter = null;
                FrameExecution= null;
                _newFlowParameter.Type = SelectedFlowParameterType;
                NavigateToNewFlowParameterDetailPage(_newFlowParameter);
            }
        }

        [RelayCommand]
        private void OnFlowTypeSelectionChanged()
        {
        }


        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

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
                FrameFlowParameter = null;
            }
        }
        private void NavigateToNewFlowParameterDetailPage(FlowParameter newflowParameter)
        {
            IFlowParameterDetailPage? page = _flowParameterPageFactory.TryGetValue(SelectedFlowParameterType, out Lazy<IFlowParameterDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadNewFlowParameter(newflowParameter);
                FrameFlowParameter = page;
            }
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FrameFlowStep = page;
            }
        }

        private void NavigateToFlowDetailPage(int id)
        {
            IFlowDetailPage? page = _flowPageFactory.TryGetValue(SelectedFlowType, out Lazy<IFlowDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowId(id);
                FrameFlow = page;
            }
        }

        private void NavigateToExecutionDetailPage(Execution execution)
        {
            IExecutionPage? page = _executionFlowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IExecutionPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.SetExecution(execution);
                FrameExecution = page;
            }
        }
    }
}