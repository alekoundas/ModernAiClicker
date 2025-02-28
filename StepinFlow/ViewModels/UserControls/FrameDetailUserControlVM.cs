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
using StepinFlow.Views.Pages.FlowParameterDetail;
using StepinFlow.Views.Pages.FlowStepDetail;
using System.ComponentModel;
using System.Windows;

namespace StepinFlow.ViewModels.UserControls
{
    public partial class FrameDetailUserControlVM : ObservableObject, INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private readonly IServiceProvider _serviceProvider;


        public event OnSaveFlowStep? OnSaveFlowStepEvent;
        public delegate void OnSaveFlowStep(int Id);

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
        private FlowTypesEnum _selectedFlowType = FlowTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowTypesEnum> _flowTypes = Enum.GetValues(typeof(FlowTypesEnum)).Cast<FlowTypesEnum>().ToList();
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
        private readonly Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>> _flowPageFactory;
        private FlowStep? _newFlowStep = null;
        private FlowParameter? _newFlowParameter = null;

        public FrameDetailUserControlVM(IDataService dataService, IServiceProvider serviceProvider)
        {
            _dataService = dataService;
            _serviceProvider = serviceProvider;

            FlowTypes = Enum.GetValues(typeof(FlowTypesEnum)).Cast<FlowTypesEnum>().ToList();
            FlowStepTypes = Enum.GetValues(typeof(FlowStepTypesEnum))
                .Cast<FlowStepTypesEnum>()
                .Where(x => x != FlowStepTypesEnum.SUCCESS)
                .Where(x => x != FlowStepTypesEnum.FAILURE)
                .Where(x => x != FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
                .Where(x => x != FlowStepTypesEnum.NEW)
                .Where(x => x != FlowStepTypesEnum.FLOW_PARAMETERS)
                .Where(x => x != FlowStepTypesEnum.FLOW_STEPS)
                .ToList();

            FlowParameterTypes = Enum.GetValues(typeof(FlowParameterTypesEnum))
                .Cast<FlowParameterTypesEnum>()
                .Where(x => x != FlowParameterTypesEnum.NEW)
                .Where(x => x != FlowParameterTypesEnum.FLOW_PARAMETERS)
                .ToList();


            // Lazy load the instances needed and not all at once.
            _flowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IFlowStepDetailPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WaitForTemplateFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_RELOCATE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorRelocateFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_CLICK, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorClickFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_SCROLL, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorScrollFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WaitFlowStepPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) },
                { FlowStepTypesEnum.SUB_FLOW_STEP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<SubFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>>
            {
                { FlowTypesEnum.FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.SUB_FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.NO_SELECTION, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            };

            _executionFlowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IExecutionPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WaitForTemplateExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_RELOCATE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorRelocateExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_CLICK, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorClickExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_SCROLL, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorScrollExecutionPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WaitExecutionPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<GoToExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowResizeExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowMoveExecutionPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<LoopExecutionPage>()) },
                { FlowStepTypesEnum.SUB_FLOW_STEP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<SubFlowStepExecutionPage>()) }
            };

            _flowParameterPageFactory = new Dictionary<FlowParameterTypesEnum, Lazy<IFlowParameterDetailPage>>
            {
                { FlowParameterTypesEnum.TEMPLATE_SEARCH_AREA, new Lazy<IFlowParameterDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchAreaFlowParameterPage>()) },
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
            FrameExecution = null;
            FrameFlowParameter = null;
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
            FrameExecution = null;
            FrameFlowParameter = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            FrameFlow = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            // Navigate to existing flow step.
            FlowStepTypesEnum? Type = await _dataService.FlowSteps.Query
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
            FrameExecution = null;
            // Navigate to existing flow step.
            FlowParameterTypesEnum? type = await _dataService.FlowParameters.Query
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
                NavigateToFlowParameterDetailPage(id);
            }

        }
        public async Task NavigateToFlow(int id)
        {
            FrameFlowStep = null;
            FrameFlowParameter = null;
            FrameExecution = null;
            // Navigate to existing flow.
            FlowTypesEnum? flowType = await _dataService.Flows.Query
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
            if (execution.FlowStep != null)
            {
                SelectedFlowStepType = execution.FlowStep.Type;
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
                FrameFlowParameter = null;
                FrameExecution = null;
                _newFlowStep.Type = SelectedFlowStepType;
                NavigateToNewFlowStepDetailPage(_newFlowStep);
            }
            else if (_newFlowParameter != null)
            {
                FrameFlow = null;
                FrameFlowStep = null;
                FrameExecution = null;
                _newFlowParameter.Type = SelectedFlowParameterType;
                NavigateToNewFlowParameterDetailPage(_newFlowParameter);
            }
        }



        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {

                page.ViewModel.LoadNewFlowStep(newFlowStep);
                page.ViewModel.OnSave -= HandleSave;
                page.ViewModel.OnSave += HandleSave;
                FrameFlowStep = page;
            }
        }
        private void NavigateToNewFlowParameterDetailPage(FlowParameter newflowParameter)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IFlowParameterDetailPage? page = _flowParameterPageFactory.TryGetValue(SelectedFlowParameterType, out Lazy<IFlowParameterDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {
                page.ViewModel.LoadNewFlowParameter(newflowParameter);
                FrameFlowParameter = page;
            }
        }
        private void NavigateToFlowParameterDetailPage(int id)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IFlowParameterDetailPage? page = _flowParameterPageFactory.TryGetValue(SelectedFlowParameterType, out Lazy<IFlowParameterDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {
                page.ViewModel.LoadFlowParameterId(id);
                FrameFlowParameter = page;
            }
            else
                FrameFlowParameter = null;

        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                page.ViewModel.OnSave -= HandleSave;
                page.ViewModel.OnSave += HandleSave;
                FrameFlowStep = page;
            }
            else
                FrameFlowStep = null;
        }

        private void NavigateToFlowDetailPage(int id)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IFlowDetailPage? page = _flowPageFactory.TryGetValue(SelectedFlowType, out Lazy<IFlowDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {
                page.ViewModel.LoadFlowId(id);
                FrameFlow = page;
            }
            else
                FrameFlow = null;
        }

        private void NavigateToExecutionDetailPage(Execution execution)
        {
            FrameFlowStep?.ViewModel.OnPageExit();
            IExecutionPage? page = _executionFlowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IExecutionPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page != null)
            {
                page.ViewModel.SetExecution(execution);
                FrameExecution = page;
            }
            else
                FrameExecution = null;
        }



        private void HandleSave(int id)
        {
            OnSaveFlowStepEvent?.Invoke(id);
        }
    }
}