using Business.Interfaces;
using Business.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using Model.Models;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Forms;
using System.Management;
using AutoMapper;
using Model.Enums;
using Model.Business;

namespace Business.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class SystemService : ISystemService
    {
        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out Model.Structs.Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(int hWnd, out Model.Structs.Rectangle lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        //[DllImport("user32.dll")]
        //public static extern bool GetCursorPos(out Point lpPoint);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        public void CursorClick(MouseButtonsEnum mouseButtonEnum)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;

            if (mouseButtonEnum == MouseButtonsEnum.RIGHT_BUTTON)
            {
                mouse_event(0x08, x, y, 0, 0);
                Thread.Sleep(100);
                mouse_event(0x010, x, y, 0, 0);
            }
            else if (mouseButtonEnum == MouseButtonsEnum.LEFT_BUTTON)
            {
                mouse_event(0x02, x, y, 0, 0);
                Thread.Sleep(100);
                mouse_event(0x04, x, y, 0, 0);
            }

        }




        public void GetDpi(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
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

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;

            if (width <= 0 && height <= 0)
                return null;

            //Take screenshot
            using (Bitmap bmp = new Bitmap(Math.Abs(width), height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(rectangle.Left, rectangle.Top, 0, 0, new System.Drawing.Size(width, height));
                    bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                    graphics.Dispose();
                    bmp.Dispose();
                }
            }

            //Return it
            Bitmap img = (Bitmap)Image.FromFile(filePath);
            return img;


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
            Model.Structs.Rectangle windowRectangle = new Model.Structs.Rectangle();

            windowRectangle.Top = SystemInformation.VirtualScreen.Top;
            windowRectangle.Bottom = SystemInformation.VirtualScreen.Bottom;
            windowRectangle.Left = SystemInformation.VirtualScreen.Left;
            windowRectangle.Right = SystemInformation.VirtualScreen.Right;


            return windowRectangle;
        }

        public ImageSizeResult GetImageSize(string imagePath)
        {
            ImageSizeResult imageSizeResult = new ImageSizeResult();

            using (var image = new Bitmap(imagePath))
            {
                imageSizeResult.Height = image.Height;
                imageSizeResult.Width = image.Width;
            }

            return imageSizeResult;
        }

        public void SetCursorPossition(Model.Structs.Point point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public async Task UpdateFlowsJSON(List<Flow> flows)
        {
            var mapper = new MapperConfiguration(x =>
            {
                x.CreateMap<Flow, FlowDto>();
                x.CreateMap<FlowStep, FlowStepDto>();
                x.CreateMap<Execution, ExecutionDto>();
            }
            ).CreateMapper();


            List<FlowDto> flowsDto = mapper.Map<List<FlowDto>>(flows);


            string json = JsonConvert.SerializeObject(flowsDto, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "Flows.json");
            await File.WriteAllTextAsync(filePath, json);
        }

        //public async Task UpdateFlowsJSON(List<Flow> flows)
        //{

        //    string json = JsonConvert.SerializeObject(flows, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    });

        //    string filePath = Path.Combine(PathHelper.GetAppDataPath(), "Flows.json");
        //    await File.WriteAllTextAsync(filePath, json);
        //}

        public List<Flow>? LoadFlowsJSON()
        {
            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "Flows.json");
            string flowsJSON = File.ReadAllText(filePath);
            if (flowsJSON != null)
            {
                var JsonFlows = JsonConvert.DeserializeObject<List<Flow>>(flowsJSON);
                if (JsonFlows != null && JsonFlows.Count > 0)
                    return JsonFlows;
            }

            return null;
        }


        public void GetScalingFactor()
        {
            List<double> physicalWidths = new List<double>();

            //Get physical width for each monitor
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");

            foreach (ManagementObject monitor in searcher.Get())
            {
                //Get the physical width (inch)
                double width = (byte)monitor["MaxHorizontalImageSize"] / 2.54;
                physicalWidths.Add(width);
            }

            //Get screen info for each monitor
            Screen[] screenList = Screen.AllScreens;
            int i = 0;

            foreach (Screen screen in screenList)
            {
                //Get the physical width (pixel)
                double physicalWidth;
                if (i < physicalWidths.Count)
                {
                    //Get the DPI
                    uint x, y;
                    GetDpi2(screen, DpiType.Effective, out x, out y);

                    //Convert inch to pixel
                    physicalWidth = physicalWidths[i] * x;
                }
                else
                {
                    physicalWidth = SystemParameters.PrimaryScreenWidth;
                }
                i++;

                //Calculate the scaling
                double scaling = 100 * (physicalWidth / screen.Bounds.Width);
                double scalingFactor = physicalWidth / screen.Bounds.Width;

                //Output the result
                Console.WriteLine(scalingFactor);
            }
        }

        public void GetDpi2(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        public List<double> GetScalingFactor2()
        {
            List<double> physicalWidths = new List<double>();
            List<double> monitorsfactor = new List<double>();

            //Get physical width for each monitor
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");

            foreach (ManagementObject monitor in searcher.Get())
            {
                //Get the physical width (inch)
                double width = (byte)monitor["MaxHorizontalImageSize"] / 2.54;
                physicalWidths.Add(width);
            }

            //Get screen info for each monitor
            Screen[] screenList = Screen.AllScreens;
            int i = 0;

            foreach (Screen screen in screenList)
            {
                //Get the physical width (pixel)
                double physicalWidth;
                if (i < physicalWidths.Count)
                {
                    //Get the DPI
                    uint x, y;
                    GetDpi3(screen, DpiType.Effective, out x, out y);

                    //Convert inch to pixel
                    physicalWidth = physicalWidths[i] * x;
                }
                else
                {
                    physicalWidth = SystemParameters.PrimaryScreenWidth;
                }
                i++;

                //Calculate the scaling
                double scaling = 100 * (physicalWidth / screen.Bounds.Width);
                double scalingFactor = physicalWidth / screen.Bounds.Width;

                //Output the result
                Console.WriteLine(scalingFactor);
                monitorsfactor.Add(scalingFactor);
            }
            return monitorsfactor;
        }

        public void GetDpi3(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

    }
}
