using CommunityToolkit.Mvvm.ComponentModel;
using Model.Enums;
using System.Collections.ObjectModel;

namespace Model.Models
{
    public partial class FlowParameter : ObservableObject
    {
        [ObservableProperty]
        public int id;

        [ObservableProperty]
        public string _name = string.Empty;

        [ObservableProperty]
        public bool _isExpanded;

        [ObservableProperty]
        public bool _isSelected;

        [ObservableProperty]
        public int _orderingNum;

        [ObservableProperty]
        public FlowParameterTypesEnum _type;

        [ObservableProperty]
        public TemplateSearchAreaTypesEnum? _templateSearchAreaType;

        [ObservableProperty]
        public string _processName = string.Empty;

        [ObservableProperty]
        public string _systemMonitorDeviceName = string.Empty;


        [ObservableProperty]
        public int _locationTop;

        [ObservableProperty]
        public int _locationLeft;

        [ObservableProperty]
        public int _locationRight;

        [ObservableProperty]
        public int _locationBottom;



        public int? FlowId { get; set; }
        public virtual Flow? Flow { get; set; }

        public int? ParentFlowParameterId { get; set; }
        public virtual FlowParameter? ParentFlowParameter { get; set; }

        public virtual ObservableCollection<FlowStep> FlowSteps { get; set; } = new ObservableCollection<FlowStep>();
        public virtual ObservableCollection<FlowParameter> ChildrenFlowParameters { get; set; } = new ObservableCollection<FlowParameter>();
    }
}
