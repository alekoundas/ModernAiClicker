using Model.Enums;

namespace Model.Models
{
    public class AppSetting
    {
        public int Id { get; set; }
        public AppSettingsEnum Key { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
