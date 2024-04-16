using System.Diagnostics;

namespace Business.Helpers
{
    public static class SystemProcessHelper
    {
        public static List<string> GetProcesses()
        {
            return Process
                .GetProcesses()
                .Select(x => x.ProcessName)
                .ToList();
        }

        public static List<string> GetProcessWindowTitles()
        {
            return Process
                .GetProcesses()
                .Where(process => !String.IsNullOrEmpty(process.MainWindowTitle))
                .Select(x => x.MainWindowTitle)
                .ToList();
        }

    }
}
