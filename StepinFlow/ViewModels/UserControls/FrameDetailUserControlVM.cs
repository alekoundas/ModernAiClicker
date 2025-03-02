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

        //[ObservableProperty]
        private IFlowStepDetailPage? _frameFlowStep;
        //[ObservableProperty]
        //private IFlowParameterDetailPage? _frameFlowParameter;
        //[ObservableProperty]
        //private IFlowDetailPage? _frameFlow;
        //[ObservableProperty]
        //private IExecutionPage? _frameExecution;
        [ObservableProperty]
        private IDetailPage? _frame;

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IDetailPage>> _flowStepPageFactory;
        private readonly Dictionary<FlowParameterTypesEnum, Lazy<IDetailPage>> _flowParameterPageFactory;
        private readonly Dictionary<FlowStepTypesEnum, Lazy<IDetailPage>> _executionFlowStepPageFactory;
        private readonly Dictionary<FlowTypesEnum, Lazy<IDetailPage>> _flowPageFactory;
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
            _flowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IDetailPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WaitForTemplateFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_RELOCATE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorRelocateFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_CLICK, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorClickFlowStepPage>()) },
                { FlowStepTypesEnum.CURSOR_SCROLL, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorScrollFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WaitFlowStepPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) },
                { FlowStepTypesEnum.SUB_FLOW_STEP, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<SubFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IDetailPage>>
            {
                { FlowTypesEnum.FLOW, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.SUB_FLOW, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.NO_SELECTION, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            };

            _executionFlowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IDetailPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchExecutionPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WaitForTemplateExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_RELOCATE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorRelocateExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_CLICK, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorClickExecutionPage>()) },
                { FlowStepTypesEnum.CURSOR_SCROLL, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<CursorScrollExecutionPage>()) },
                { FlowStepTypesEnum.WAIT, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WaitExecutionPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<GoToExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeExecutionPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveExecutionPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<LoopExecutionPage>()) },
                { FlowStepTypesEnum.SUB_FLOW_STEP, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<SubFlowStepExecutionPage>()) }
            };

            _flowParameterPageFactory = new Dictionary<FlowParameterTypesEnum, Lazy<IDetailPage>>
            {
                { FlowParameterTypesEnum.TEMPLATE_SEARCH_AREA, new Lazy<IDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchAreaFlowParameterPage>()) },
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
            //FrameFlow = null;
            //FrameFlowStep = null;
            //FrameExecution = null;
            //FrameFlowParameter = null;
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
            Frame = null;
            //FrameFlow = null;
            //FrameFlowStep = null;
            //FrameExecution = null;
            //FrameFlowParameter = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            //FrameFlow = null;
            //FrameFlowParameter = null;
            //FrameExecution = null;
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
            //FrameFlow = null;
            //FrameFlowStep = null;
            //FrameExecution = null;
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
            //FrameFlowStep = null;
            //FrameFlowParameter = null;
            //FrameExecution = null;
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
            //FrameFlow = null;
            //FrameFlowStep = null;
            //FrameFlowParameter = null;
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
                //FrameFlow = null;
                //FrameFlowParameter = null;
                //FrameExecution = null;
                _newFlowStep.Type = SelectedFlowStepType;
                NavigateToNewFlowStepDetailPage(_newFlowStep);
            }
            else if (_newFlowParameter != null)
            {
                //FrameFlow = null;
                //FrameFlowStep = null;
                //FrameExecution = null;
                _newFlowParameter.Type = SelectedFlowParameterType;
                NavigateToNewFlowParameterDetailPage(_newFlowParameter);
            }
        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {

        }


        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.FlowStepViewModel != null)
            {
                page.FlowStepViewModel.LoadNewFlowStep(newFlowStep);
                page.FlowStepViewModel.OnSave -= HandleSave;
                page.FlowStepViewModel.OnSave += HandleSave;
                Frame = page;
            }
            else
                Frame = null;
        }
        private void NavigateToNewFlowParameterDetailPage(FlowParameter newflowParameter)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _flowParameterPageFactory.TryGetValue(SelectedFlowParameterType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.FlowParameterViewModel != null)
            {
                page.FlowParameterViewModel.LoadNewFlowParameter(newflowParameter);
                Frame= page;
            }
            else
                Frame = null;
        }
        private void NavigateToFlowParameterDetailPage(int id)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _flowParameterPageFactory.TryGetValue(SelectedFlowParameterType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.FlowParameterViewModel != null)
            {
                page.FlowParameterViewModel.LoadFlowParameterId(id);
                Frame = page;
            }
            else
                Frame= null;
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.FlowStepViewModel != null)
            {
                page.FlowStepViewModel.LoadFlowStepId(id);
                page.FlowStepViewModel.OnSave -= HandleSave;
                page.FlowStepViewModel.OnSave += HandleSave;
                Frame = page;
            }
            else
                Frame = null;
        }

        private void NavigateToFlowDetailPage(int id)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _flowPageFactory.TryGetValue(SelectedFlowType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.FlowViewModel != null)
            {
                page.FlowViewModel.LoadFlowId(id);
                Frame = page;
            }
            else
                Frame = null;
        }

        private void NavigateToExecutionDetailPage(Execution execution)
        {
            Frame?.FlowStepViewModel?.OnPageExit();
            IDetailPage? page = _executionFlowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IDetailPage>? lazzyPage) ? lazzyPage.Value : null;
            if (page?.ExecutionViewModel != null)
            {
                page.ExecutionViewModel.SetExecution(execution);
                Frame = page;
            }
            else
                Frame = null;
        }



        private void HandleSave(int id)
        {
            OnSaveFlowStepEvent?.Invoke(id);
        }
    }
}