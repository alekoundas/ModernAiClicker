﻿using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using Model.Structs;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class FlowStepDto : ObservableObject
    {
        public int Id;

        public string Name = "";

        public string ProcessName = string.Empty;

        public bool IsExpanded = true;

        public int OrderingNum;

        public FlowStepTypesEnum FlowStepType;

        public bool Disabled { get; set; }

        public int? FlowId { get; set; }
        public int? ParentFlowStepId { get; set; }

        public virtual ObservableCollection<FlowStepDto> ChildrenFlowSteps { get; set; } = new ObservableCollection<FlowStepDto>();
        public virtual ObservableCollection<ExecutionDto> Executions { get; set; } = new ObservableCollection<ExecutionDto>();





        //TODO create tables for the bellow

        // Template search
        public string TemplateImagePath = "";

        public double Accuracy = 0.00d;
        public Point ResultLocation { get; set; }


        // Mouse
        public MouseActionsEnum MouseAction;

        public MouseButtonsEnum MouseButton;

        public bool MouseLoopInfinite;

        public int? MouseLoopTimes;

        public int? MouseLoopDebounceTime;

        public TimeOnly? MouseLoopTime;

        public int? ParentTemplateSearchFlowStepId;
    }
}