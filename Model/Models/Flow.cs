using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model.Models
{
    public partial class Flow : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool Favorite { get; set; }
        public bool IsNew { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public int? ExecutionId { get; set; }
        public virtual Execution? Execution { get; set; }

        public virtual ObservableCollection<FlowStep> FlowSteps { get; set; } = new ObservableCollection<FlowStep>();

        public Flow CreateClone()
        {
            return new Flow()
            {
                Id = Id,
                Favorite = Favorite,
                Name = Name,
                FlowSteps = FlowSteps,
                IsNew = IsNew,
                IsExpanded = IsExpanded,
                IsSelected = IsSelected,
            };
        }
    }

    public partial class FlowDto
    {
        public int Id;
        public string Name { get; set; } = string.Empty;

        public bool Favorite { get; set; }
        public bool IsNew { get; set; }

        public ObservableCollection<FlowStepDto> FlowSteps { get; set; } = new ObservableCollection<FlowStepDto>();

    }

}
