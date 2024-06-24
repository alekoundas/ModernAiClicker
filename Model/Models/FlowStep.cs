using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public string _templateImagePath = "";

        [ObservableProperty]
        public bool _isExpanded = true;

        [ObservableProperty]
        public int _orderingNum;


        [ObservableProperty]
        public FlowStepTypesEnum _flowStepType;

        public double Accuracy { get; set; } = 0.00d;
        public bool Disabled { get; set; }

        public int? FlowId { get; set; }
        public virtual Flow? Flow { get; set; }

        public int? ExecutionId { get; set; }
        public virtual Execution? Execution { get; set; }

        public int? ParentFlowStepId { get; set; }
        public virtual FlowStep? ParentFlowStep { get; set; }

        public virtual ObservableCollection<FlowStep>? ChildrenFlowSteps { get; set; }

        //TODO create tables for the bellow

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
    }

    public partial class FlowStepDto
    {
        public int id;

        public string Name { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string TemplateImagePath { get; set; } = "";
        public double Accuracy { get; set; } = 0.00d;
        public bool Status { get; set; }
        public bool Disabled { get; set; }
        public bool IsNew { get; set; }

        public FlowStepTypesEnum FlowStepType { get; set; }
        public FlowStepActionsFoundEnum FlowStepActionsFound { get; set; }
        public FlowStepActionsNotFoundEnum FlowStepActionsNotFound { get; set; }
        [JsonIgnore]
        public int FlowId { get; set; }
        [JsonIgnore]
        public FlowDto Flow { get; set; } = new FlowDto();
    }

}
