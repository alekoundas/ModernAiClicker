namespace Business.Helpers
{
    public static class PathHelper
    {
        public static string GetAppDataPath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataFolder = Path.Combine(localAppData, "ModernAiClicker");

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            return appDataFolder;
        }
    }
}
