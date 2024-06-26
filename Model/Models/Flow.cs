using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model.Models
{
    public partial class Flow : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsSelected { get; set; }

        [ObservableProperty]
        public bool _isExpanded = true;

        [ObservableProperty]
        public int _orderingNum;

        public virtual ObservableCollection<FlowStep> FlowSteps { get; set; } = new ObservableCollection<FlowStep>();
        public virtual ObservableCollection<Execution>? Executions { get; set; } = new ObservableCollection<Execution>();

    }
}
