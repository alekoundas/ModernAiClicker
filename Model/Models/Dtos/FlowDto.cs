using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class FlowDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        public bool IsExpanded = true;

        public int OrderingNum;

        public virtual ObservableCollection<FlowStepDto> FlowSteps { get; set; }
        public virtual ObservableCollection<ExecutionDto>? Executions { get; set; }

    }
}
