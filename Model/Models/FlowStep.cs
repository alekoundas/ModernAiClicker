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

        public string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public string _processName = string.Empty;

        [ObservableProperty]
        public string _templateImagePath = "";
        public double Accuracy { get; set; } = 0.00d;
        public bool Status { get; set; }
        public bool Disabled { get; set; }
        public bool IsNew { get; set; }

        public FlowStepTypesEnum FlowStepType { get; set; }
        public FlowStepActionsFoundEnum FlowStepActionsFound { get; set; }
        public FlowStepActionsNotFoundEnum FlowStepActionsNotFound { get; set; }

        public int? FlowId { get; set; }

        public virtual Flow? Flow { get; set; }

        public int? ExecutionId { get; set; }
        public virtual Execution? Execution { get; set; }


        public int? ParentFlowStepId { get; set; }

        public virtual FlowStep? ParentFlowStep { get; set; }

        public virtual ObservableCollection<FlowStep>? ChildrenFlowSteps { get; set; } 

        public FlowStep Clone()
        {
            return new FlowStep()
            {
                Id = Id,
                id = Id,
                Name = Name,
                ProcessName = ProcessName,
                TemplateImagePath = TemplateImagePath,
                Accuracy = Accuracy,
                Status = Status,
                Disabled = Disabled,
                IsNew = IsNew,
                Flow = Flow,
                FlowId = FlowId,
                FlowStepActionsFound = FlowStepActionsFound,
                FlowStepActionsNotFound = FlowStepActionsNotFound,
                FlowStepType = FlowStepType,
            };
        }

        public FlowStep CreateModel()
        {
            return new FlowStep()
            {
                FlowId = FlowId,
                Flow = null,
                Name = Name,
                ProcessName = ProcessName,
                TemplateImagePath = TemplateImagePath,
                Accuracy = Accuracy,
                Status = Status,
                Disabled = Disabled,
                IsNew = IsNew,
                FlowStepActionsFound = FlowStepActionsFound,
                FlowStepActionsNotFound = FlowStepActionsNotFound,
                FlowStepType = FlowStepType,
            };
        }

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
