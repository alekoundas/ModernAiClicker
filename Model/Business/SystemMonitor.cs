using CommunityToolkit.Mvvm.ComponentModel;

namespace Model.Business
{
    public partial class SystemMonitor : ObservableObject
    {
        [ObservableProperty]
        public string _deviceName= string.Empty;

        [ObservableProperty]
        public int _left; 

        [ObservableProperty]
        public int _top; 

        [ObservableProperty]
        public int _right;

        [ObservableProperty]
        public int _bottom;
    }
}
