using Business.Interfaces;
using Business.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Model.Structs;
using System.Drawing;
using Model.Models;
using OpenCvSharp;
using Newtonsoft.Json;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.DirectoryServices;
using System.Windows.Forms;
using System.Data;

namespace Business.Services
{
    public class SystemService : ISystemService
    {
        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(int hWnd, out Model.Structs.Rectangle lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        //[DllImport("user32.dll")]
        //public static extern bool GetCursorPos(out Point lpPoint);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);





        public  void GetDpi(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }


        private List<string> kek(DpiType type)
        {
            var list = new List<string>();
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                uint x, y;
                GetDpi(screen, type, out x, out y);
                list.Add(screen.DeviceName + " - dpiX=" + x + ", dpiY=" + y);
            }
            return list;
        }











        public Bitmap? TakeScreenShot(Model.Structs.Rectangle rectangle, string filename = "Screenshot")
        {
            string filePath = Path.Combine(PathHelper.GetAppDataPath(), filename + ".png");

            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;

            if (width > 0 && height > 0)
            {

                //Take screenshot
                using (Bitmap bmp = new Bitmap(Math.Abs(width), height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        graphics.CopyFromScreen(rectangle.Left, rectangle.Top, 0, 0, new System.Drawing.Size(width, height));
                    }

                    bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }

                //Return it
                Bitmap img = (Bitmap)System.Drawing.Image.FromFile(filePath);

                return img;
            }

            return null;
        }

        public Model.Structs.Rectangle GetWindowSize(string processName)
        {
            var firestoneProcess = Process.GetProcessesByName(processName).FirstOrDefault();
            if (firestoneProcess != null)
            {

                var firestoneHandle = firestoneProcess.MainWindowHandle.ToInt32();

                GetWindowRect(firestoneHandle, out Model.Structs.Rectangle windowRectangle);

                return windowRectangle;
            }

            return new Model.Structs.Rectangle();
        }


        public Model.Structs.Rectangle GetScreenSize()
        {
            var aa1 = kek(DpiType.Effective);
            var aa2 = kek(DpiType.Raw);
            var aa3 = kek(DpiType.Angular);
            Model.Structs.Rectangle windowRectangle = new Model.Structs.Rectangle();

            windowRectangle.Top = SystemInformation.VirtualScreen.Top;
            windowRectangle.Bottom = SystemInformation.VirtualScreen.Bottom;
            windowRectangle.Left = SystemInformation.VirtualScreen.Left;
            windowRectangle.Right = SystemInformation.VirtualScreen.Right;


            return windowRectangle;
        }

        public void SetCursorPossition(Model.Structs.Rectangle rectangle)
        {
            int x = rectangle.Left;
            int y = rectangle.Top;

            SetCursorPos(x, y);
        }


        public async Task UpdateFlowsJSON(List<Flow> flows)
        {

            string json = JsonConvert.SerializeObject(flows, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //string flowsJSON = JsonConvert.SerializeObject(flows);
            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "Flows.json");
            await File.WriteAllTextAsync(filePath, json);
        }

        public List<Flow>? LoadFlowsJSON()
        {
            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "Flows.json");
            string flowsJSON = File.ReadAllText(filePath);
            if (flowsJSON != null)
            {
                List<Flow> flows = JsonConvert.DeserializeObject<List<Flow>>(flowsJSON);

                return flows;
            }

            return null;
        }
    }
}
