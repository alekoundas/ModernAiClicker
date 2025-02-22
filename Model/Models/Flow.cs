using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class Flow : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public string _name = "";

        [ObservableProperty]
        public FlowStepTypesEnum _type = FlowStepTypesEnum.NO_SELECTION;

        [ObservableProperty]
        public bool _isSelected = true;

        [ObservableProperty]
        public bool _isExpanded = true;

        [ObservableProperty]
        public int _orderingNum;


        public int? FlowStepId { get; set; }
        public virtual FlowStep? FlowStep { get; set; }

        public int? FlowParameterId { get; set; }
        public virtual FlowParameter? FlowParameter { get; set; }


        public virtual ObservableCollection<FlowStep> FlowSteps { get; set; } = new ObservableCollection<FlowStep>();
        public virtual ObservableCollection<Execution> Executions { get; set; } = new ObservableCollection<Execution>();
        public virtual ObservableCollection<FlowParameter> FlowParameters { get; set; } = new ObservableCollection<FlowParameter>();
    }
}
