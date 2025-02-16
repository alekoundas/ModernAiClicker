using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model.Enums;
using Model.Models;
using StepinFlow.Views.Pages.FlowDetail;
using StepinFlow.Views.Pages.FlowStepDetail;
using System;
using System.Collections.ObjectModel;
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
        private bool _isFlowStepTypeEnabled = false;
        [ObservableProperty]
        private Visibility _flowStepTypeVisibility = Visibility.Collapsed;


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

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IFlowStepDetailPage>> _flowStepPageFactory;
        private readonly Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>> _flowPageFactory;
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
                { FlowStepTypesEnum.SLEEP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<SleepFlowStepPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IFlowStepDetailPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IFlowDetailPage>>
            {
                { FlowTypesEnum.FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.SUB_FLOW, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
                { FlowTypesEnum.NO_SELECTION, new Lazy<IFlowDetailPage>(() => serviceProvider.GetRequiredService<FlowPage>()) },
            };
        }

        public void NavigateToNewFlowStep(FlowStep newFlowStep)
        {
            // Navigate to new flow step.
            IsFlowStepTypeEnabled = true;
            FlowStepTypeVisibility = Visibility.Visible;
            FlowTypeVisibility = Visibility.Collapsed;
            SelectedFlowStepType = FlowStepTypesEnum.NO_SELECTION;
            _newFlowStep = newFlowStep;
            FrameFlowStep = null;
        }

        public async Task NavigateToFlowStep(int id)
        {
            // Navigate to existing flow step.
            FlowStepTypesEnum? flowStepType = await _baseDatawork.FlowSteps.Query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.FlowStepType)
                .FirstOrDefaultAsync();

            if (flowStepType != null)
            {
                SelectedFlowStepType = flowStepType.Value;
                FlowStepTypeVisibility = Visibility.Visible;
                FlowTypeVisibility = Visibility.Collapsed;
                IsFlowStepTypeEnabled = false;
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
                FlowStepTypeVisibility = Visibility.Collapsed;
                FlowTypeVisibility = Visibility.Visible;
                NavigateToFlowDetailPage(id);
            }

        }



        [RelayCommand]
        private void OnFlowStepTypeSelectionChanged()
        {
            if (_newFlowStep != null)
            {
                FrameFlow = null;
                _newFlowStep.FlowStepType = SelectedFlowStepType;
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
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadNewFlowStep(newFlowStep);
                FrameFlowStep = page;
            }
            else
                FrameFlowStep = null;
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            FrameFlow = null;
            IFlowStepDetailPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IFlowStepDetailPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FrameFlowStep = page;
            }
            else
                FrameFlowStep = null;
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
                FrameFlow = null;
        }

    }
}