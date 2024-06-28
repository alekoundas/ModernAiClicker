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
        public bool _isExpanded = true;

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

        public virtual ObservableCollection<FlowStep>? ChildrenFlowSteps { get; set; }
        public virtual ObservableCollection<Execution>? Executions { get; set; }







        //TODO create tables for the bellow

        // Template search
        [ObservableProperty]
        public string _templateImagePath = "";

        [ObservableProperty]
        public double _accuracy = 0.00d;

        public Point ResultLocation { get; set; }


        // Mouse
        [ObservableProperty]
        public MouseActionsEnum _mouseAction;

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

        public int? ParentTemplateSearchFlowStepId { get; set; }
        public virtual FlowStep? ParentTemplateSearchFlowStep { get; set; }
        public virtual ObservableCollection<FlowStep>? ChildrenTemplateSearchFlowSteps { get; set; }

    }
}
