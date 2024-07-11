using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class Flow : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public string _name = "";

        [ObservableProperty]
        public bool _isSelected = true;

        [ObservableProperty]
        public bool _isExpanded = true;

        [ObservableProperty]
        public int _orderingNum;

        public virtual ObservableCollection<FlowStep> FlowSteps { get; set; } = new ObservableCollection<FlowStep>();
        public virtual ObservableCollection<Execution> Executions { get; set; } = new ObservableCollection<Execution>();
    }
}
