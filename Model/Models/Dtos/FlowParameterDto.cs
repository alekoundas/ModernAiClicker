
using Model.Enums;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class FlowParameterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public int OrderingNum { get; set; }
        public FlowParameterTypesEnum Type { get; set; }
        public TemplateSearchAreaTypesEnum? TemplateSearchAreaType { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string SystemMonitorDeviceName { get; set; } = string.Empty;
        public int LocationTop { get; set; }
        public int LocationLeft { get; set; }
        public int LocationRight { get; set; }
        public int LocationBottom { get; set; }

        public int? FlowId { get; set; }
        public int? ParentFlowParameterId { get; set; }

        public virtual List<FlowStepDto> FlowSteps { get; set; } = new List<FlowStepDto>();
        public virtual List<FlowParameterDto> ChildrenFlowParameters { get; set; } = new List<FlowParameterDto>();
    }
}
