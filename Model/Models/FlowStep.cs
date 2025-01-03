﻿using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using Model.Structs;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class FlowStep : ObservableObject
    {
        [ObservableProperty]
        public int id;

        [ObservableProperty]
        public string _name = "";

        [ObservableProperty]
        public string _processName = string.Empty;

        [ObservableProperty]
        public bool _isExpanded;

        [ObservableProperty]
        public bool _isSelected = true;

        [ObservableProperty]
        public int _orderingNum;

        [ObservableProperty]
        public FlowStepTypesEnum _flowStepType;


        public bool Disabled { get; set; }

        public int? FlowId { get; set; }
        public virtual Flow? Flow { get; set; }

        public int? ParentFlowStepId { get; set; }
        public virtual FlowStep? ParentFlowStep { get; set; }

        public virtual ObservableCollection<FlowStep> ChildrenFlowSteps { get; set; } = new ObservableCollection<FlowStep>();
        public virtual ObservableCollection<Execution>? Executions { get; set; }  = new ObservableCollection<Execution>();
        public virtual ObservableCollection<Execution>? CurrentMultipleTemplateSearchFlowStepExecutions { get; set; }  = new ObservableCollection<Execution>();







        //TODO create tables for the bellow

        // Template search
        [ObservableProperty]
        public string _templateImagePath = "";

        [ObservableProperty]
        public byte[]? _templateImage;

        [ObservableProperty]
        public decimal _accuracy = 0.00m;

        [ObservableProperty]
        public int _locationX;

        [ObservableProperty]
        public int _locationY;

        [ObservableProperty]
        public int _maxLoopCount;

        [ObservableProperty]
        public bool _removeTemplateFromResult;

        [ObservableProperty]
        public string _loopResultImagePath = ""; // Only used for test action in MultipleTemplateSeaarchLoop vm.   TODO Check if needed!

        // Mouse
        [ObservableProperty]
        public MouseActionsEnum _mouseAction;

        [ObservableProperty]
        public MouseScrollDirectionEnum _mouseScrollDirectionEnum;

        [ObservableProperty]
        public MouseButtonsEnum _mouseButton;

        [ObservableProperty]
        public bool _mouseLoopInfinite;

        [ObservableProperty]
        public int? _mouseLoopTimes;

        [ObservableProperty]
        public int? _mouseLoopDebounceTime;

        [ObservableProperty]
        public TimeOnly? _mouseLoopTime;

        //System
        [ObservableProperty]
        public int? _sleepForHours;

        [ObservableProperty]
        public int? _sleepForMinutes;

        [ObservableProperty]
        public int? _sleepForSeconds;

        [ObservableProperty]
        public int? _sleepForMilliseconds;

        // Window
        [ObservableProperty]
        public int _windowHeight;

        [ObservableProperty]
        public int _windowWidth;


        // Used by GoTo and also by "MultipleTemplateSearch"
        public int? ParentTemplateSearchFlowStepId { get; set; }
        public virtual FlowStep? ParentTemplateSearchFlowStep { get; set; }
        public virtual ObservableCollection<FlowStep> ChildrenTemplateSearchFlowSteps { get; set; } = new ObservableCollection<FlowStep>();
    }
}
