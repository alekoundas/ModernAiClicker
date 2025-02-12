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

namespace StepinFlow.ViewModels.UserControls
{
    public partial class FlowStepFrameUserControlViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IServiceProvider _serviceProvider;

        //public event NavigateToFlowStepDetailPageEvent? NavigateToFlowStepDetailPage;
        //public delegate void NavigateToFlowStepDetailPageEvent(int? id = null);

        [ObservableProperty]
        private FlowStepTypesEnum _selectedFlowStepType = FlowStepTypesEnum.NO_SELECTION;
        [ObservableProperty]
        private List<FlowStepTypesEnum> _flowStepTypes = Enum.GetValues(typeof(FlowStepTypesEnum)).Cast<FlowStepTypesEnum>().ToList();
        [ObservableProperty]
        private bool _isEnabled = false;


        [ObservableProperty]
        private IPage? _framePage;

        private readonly Dictionary<FlowStepTypesEnum, Lazy<IPage>> _pageFactory;
        private FlowStep? _newFlowStep = null;

        public FlowStepFrameUserControlViewModel(IBaseDatawork baseDatawork, IServiceProvider serviceProvider)
        {
            _baseDatawork = baseDatawork;
            _serviceProvider = serviceProvider;

            // Lazy load the instances needed and not all at once.
            _pageFactory = new Dictionary<FlowStepTypesEnum, Lazy<IPage>>
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
        }

        public void NavigateToNewFlowStep(FlowStep newFlowStep)
        {
            // Navigate to new flow step.
            IsEnabled = true;
            SelectedFlowStepType = FlowStepTypesEnum.NO_SELECTION;
            //NavigateToNewFlowStepDetailPage(newFlowStep);
            _newFlowStep = newFlowStep;

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
                IsEnabled = false;
                NavigateToFlowStepDetailPage(id);
            }

        }



        [RelayCommand]
        private void OnComboboxSelectionChanged()
        {
            if (_newFlowStep != null)
                NavigateToNewFlowStepDetailPage(_newFlowStep);
        }

        private void NavigateToFlowStepDetailPage(int id)
        {
            IPage? page = _pageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadFlowStepId(id);
                FramePage = page;
            }
        }

        private void NavigateToNewFlowStepDetailPage(FlowStep newFlowStep)
        {
            IPage? page = _pageFactory.TryGetValue(SelectedFlowStepType, out Lazy<IPage>? lazzyPage) ? lazzyPage.Value : null;

            if (page != null)
            {
                page.ViewModel.LoadNewFlowStep(newFlowStep);
                FramePage = page;
            }
        }
    }
}