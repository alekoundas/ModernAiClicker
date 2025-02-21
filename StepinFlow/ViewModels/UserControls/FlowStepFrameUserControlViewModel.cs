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
        private TypesEnum _selectedType = TypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<TypesEnum> _Types = Enum.GetValues(typeof(TypesEnum)).Cast<TypesEnum>().ToList();
        [ObservableProperty]
        private bool _isTypeEnabled = false;
        [ObservableProperty]
        private Visibility _TypeVisibility = Visibility.Collapsed;


        // Flow Type
        [ObservableProperty]
        private FlowTypesEnum _selectedFlowType = FlowTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowTypesEnum> _flowTypes = Enum.GetValues(typeof(FlowTypesEnum)).Cast<FlowTypesEnum>().ToList();
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

        private readonly Dictionary<TypesEnum, Lazy<IFlowStepDetailPage>> _flowStepPageFactory;
        private readonly Dictionary<TypesEnum, Lazy<IExecutionPage>> _executionFlowStepPageFactory;
        private readonly Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>> _flowPageFactory;
        private FlowStep? _newFlowStep = null;

        public FlowStepFrameUserControlViewModel(IBaseDatawork baseDatawork, IServiceProvider serviceProvider)
        {
            _baseDatawork = baseDatawork;
            _serviceProvider = serviceProvider;

            // Lazy load the instances needed and not all at once.
            _flowStepPageFactory = new Dictionary<TypesEnum, Lazy<IFlowStepDetailPage>>
            {
                { TypesEnum.TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchFlowStepPage>()) },
                { TypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<TemplateSearchLoopFlowStepPage>()) },
                { TypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchLoopFlowStepPage>()) },
                { TypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchFlowStepPage>()) },
                { TypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WaitForTemplateFlowStepPage>()) },
                { TypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorMoveFlowStepPage>()) },
                { TypesEnum.MOUSE_CLICK, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorClickFlowStepPage>()) },
                { TypesEnum.MOUSE_SCROLL, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<CursorScrollFlowStepPage>()) },
                { TypesEnum.SLEEP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<SleepFlowStepPage>()) },
                { TypesEnum.GO_TO, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { TypesEnum.WINDOW_RESIZE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { TypesEnum.WINDOW_MOVE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { TypesEnum.LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>>
            {
                { FlowTypesEnum.FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.SUB_FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.NO_SELECTION, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            };

            _executionFlowStepPageFactory = new Dictionary<TypesEnum, Lazy<IExecutionPage>>
            {
                { TypesEnum.TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { TypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<TemplateSearchExecutionPage>()) },
                { TypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchLoopExecutionPage>()) },
                { TypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchExecutionPage>()) },
                { TypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WaitForTemplateExecutionPage>()) },
                { TypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorMoveExecutionPage>()) },
                { TypesEnum.MOUSE_CLICK, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorClickExecutionPage>()) },
                { TypesEnum.MOUSE_SCROLL, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<CursorScrollExecutionPage>()) },
                { TypesEnum.SLEEP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<SleepExecutionPage>()) },
                { TypesEnum.GO_TO, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<GoToExecutionPage>()) },
                { TypesEnum.WINDOW_RESIZE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowResizeExecutionPage>()) },
                { TypesEnum.WINDOW_MOVE, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<WindowMoveExecutionPage>()) },
                { TypesEnum.LOOP, new Lazy<IExecutionPage>(() => serviceProvider.GetRequiredService<LoopExecutionPage>()) }
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
            SelectedType = TypesEnum.NO_SELECTION;
            _newFlowStep = newFlowStep;
            FrameFlowStep = null;
            FrameExecution = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            // Navigate to existing flow step.
            TypesEnum? Type = await _baseDatawork.FlowSteps.Query
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
            FlowTypesEnum? flowType = await _baseDatawork.Flows.Query
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