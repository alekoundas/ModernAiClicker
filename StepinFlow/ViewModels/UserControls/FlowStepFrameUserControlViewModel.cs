using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model.Enums;
using Model.Models;
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
        private IPage? _framePage;

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IPage>> _flowStepPageFactory;
        private readonly Dictionary<FlowTypesEnum, Lazy<IPage>> _flowPageFactory;
        private FlowStep? _newFlowStep = null;

        public FlowStepFrameUserControlViewModel(IBaseDatawork baseDatawork, IServiceProvider serviceProvider)
        {
            _baseDatawork = baseDatawork;
            _serviceProvider = serviceProvider;

            // Lazy load the instances needed and not all at once.
            _flowStepPageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IPage>>
            {
                { FlowStepTypesEnum.TEMPLATE_SEARCH, new Lazy<IPage>(() => serviceProvider.GetRequiredService<TemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.TEMPLATE_SEARCH_LOOP, new Lazy<IPage>(() => serviceProvider.GetRequiredService<TemplateSearchLoopFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_LOOP, new Lazy<IPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchLoopFlowStepPage>()) },
                { FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH, new Lazy<IPage>(() => serviceProvider.GetRequiredService<MultipleTemplateSearchFlowStepPage>()) },
                { FlowStepTypesEnum.WAIT_FOR_TEMPLATE, new Lazy<IPage>(() => serviceProvider.GetRequiredService<WaitForTemplateFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_MOVE_COORDINATES, new Lazy<IPage>(() => serviceProvider.GetRequiredService<CursorMoveFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_CLICK, new Lazy<IPage>(() => serviceProvider.GetRequiredService<CursorClickFlowStepPage>()) },
                { FlowStepTypesEnum.MOUSE_SCROLL, new Lazy<IPage>(() => serviceProvider.GetRequiredService<CursorScrollFlowStepPage>()) },
                { FlowStepTypesEnum.SLEEP, new Lazy<IPage>(() => serviceProvider.GetRequiredService<SleepFlowStepPage>()) },
                { FlowStepTypesEnum.GO_TO, new Lazy<IPage>(() => serviceProvider.GetRequiredService<GoToFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_RESIZE, new Lazy<IPage>(() => serviceProvider.GetRequiredService<WindowResizeFlowStepPage>()) },
                { FlowStepTypesEnum.WINDOW_MOVE, new Lazy<IPage>(() => serviceProvider.GetRequiredService<WindowMoveFlowStepPage>()) },
                { FlowStepTypesEnum.LOOP, new Lazy<IPage>(() => serviceProvider.GetRequiredService<LoopFlowStepPage>()) }
            };

            _flowPageFactory = new Dictionary<FlowTypesEnum, Lazy<IPage>>
            {
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
            FramePage = null;
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
        private void OnFlowTypeSelectionChanged()
        {
            if (_newFlowStep != null)
            {
                _newFlowStep.FlowStepType = SelectedFlowStepType;
                NavigateToNewFlowStepDetailPage(_newFlowStep);
            }
        }

        [RelayCommand]
        private void OnFlowSelectionChanged()
        {
        }
        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            IPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadNewFlowStep(newFlowStep);
                FramePage = page;
            }
            else
                FramePage = null;
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            IPage? page = _flowStepPageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FramePage = page;
            }
            else
                FramePage = null;
        }

        private void NavigateToFlowDetailPage(int id)
        {
            IPage? page = _flowPageFactory.TryGetValue(SelectedFlowType, out Lazy<IPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FramePage = page;
            }
            else
                FramePage = null;
        }

    }
}