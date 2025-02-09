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

        public static string GetExportDataPath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string exportPath = Path.Combine(localAppData, "StepinFlow", "Export");

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            return exportPath;
        }
    }
}
