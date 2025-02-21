﻿using Business.Helpers;
using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using StepinFlow.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StepinFlow.ViewModels.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionViewModel : ObservableObject, IExecutionViewModel
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IWindowService _windowService;

        [ObservableProperty]
        private Execution _execution;
        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();
        [ObservableProperty]
        private byte[]? _resultImage = null;
        [ObservableProperty] 
        private ObservableCollection<FlowStep>? _childrenTemplateSearchFlowSteps;


        public MultipleTemplateSearchExecutionViewModel(IBaseDatawork baseDatawork, IWindowService windowService)
        {
            _baseDatawork = baseDatawork;
            _windowService = windowService;
            _execution = new Execution();
        }

        public async void  SetExecution(Execution execution)
        {
            execution = await _baseDatawork.Executions.Query
                .Include(x=>x.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps)
                .FirstAsync(x=>x.Id==execution.Id);

            List<FlowStep> flowSteps = execution.FlowStep.ParentTemplateSearchFlowStep.ChildrenTemplateSearchFlowSteps
              .Where(x => x.Type == FlowStepTypesEnum.MULTIPLE_TEMPLATE_SEARCH_CHILD)
              .ToList();

            ChildrenTemplateSearchFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
            Execution = execution;

            if (execution.ResultImagePath != null)
                ResultImage = File.ReadAllBytes(execution.ResultImagePath);
        }

        [RelayCommand]
        private async Task OnTemplateImageDoubleClick(MouseButtonEventArgs e)
        {

            // Check if it's a double-click.
            if (e.ClickCount == 2)
            {
                if (e.Source is System.Windows.Controls.Image img && img.Source is BitmapSource bitmapSource)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                        encoder.Save(stream);
                        byte[]? image = await _windowService.OpenScreenshotSelectionWindow(stream.ToArray(), false);
                        if (image != null)
                            using (var stream2 = new MemoryStream(image))
                            {
                                var decoder = BitmapDecoder.Create(stream2, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                                img.Source = decoder.Frames[0];
                            }
                    }
                }

            }
        }

        [RelayCommand]
        private async Task OnResultImageDoubleClick(MouseButtonEventArgs e)
        {
            // Check if it's a double-click.
            if (e.ClickCount == 2)
                await _windowService.OpenScreenshotSelectionWindow(ResultImage, false);
        }
    }
}
