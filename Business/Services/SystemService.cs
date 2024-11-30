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
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Windows.Media.Media3D;
using OpenCvSharp;
using System.Windows.Input;
using System.Windows.Automation;
using static Business.Services.SystemService;
using System;
using Newtonsoft.Json.Linq;

namespace Business.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class SystemService : ISystemService
    {
        //public enum DpiType
        //{
        //    Effective = 0,
        //    Angular = 1,
        //    Raw = 2,
        //}
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public uint Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public int MouseData;
            public MouseFlag Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Model.Structs.Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(int hWnd, out Model.Structs.Rectangle lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(int hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);


        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        //[DllImport("User32.dll")]
        //private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        //[DllImport("Shcore.dll")]
        //private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        //public void CursorScroll()
        //{
        //    var input = new INPUT();
        //    input.Type = (UInt32)InputType.Mouse;
        //    input.Data.Mouse.Flags = MouseFlag.VerticalWheel;
        //    input.Data.Mouse.MouseData = 20;


        //    INPUT[] inputs = new INPUT[] { input };
        //    uint asdasd = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));

        //}

        //public void CursorScroll()
        //{
        //    mouse_event(MouseFlag.VerticalWheel, 0, 0, -120, 0);
        //}

        public void CursorScroll(MouseScrollDirectionEnum scrollDirection, int steps)
        {
            int wheelOrientation = 0;
            int direction = 120;
            switch (scrollDirection)
            {
                case MouseScrollDirectionEnum.UP:
                    wheelOrientation = 0x0800;
                    break;
                case MouseScrollDirectionEnum.DOWN:
                    wheelOrientation = 0x0800;
                    direction *= -1;
                    break;
                case MouseScrollDirectionEnum.LEFT:
                    wheelOrientation = 0x01000;
                    break;
                case MouseScrollDirectionEnum.RIGT:
                    wheelOrientation = 0x01000;
                    direction *= -1;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < steps; i++)
            {
                Thread.Sleep(100);
                mouse_event(wheelOrientation, 0, 0, direction, 0);
            }
        }

        [Flags]
        internal enum MouseFlag : uint // UInt32
        {
            /// <summary>
            /// Specifies that movement occurred.
            /// </summary>
            Move = 0x0001,

            /// <summary>
            /// Specifies that the left button was pressed.
            /// </summary>
            LeftDown = 0x0002,

            /// <summary>
            /// Specifies that the left button was released.
            /// </summary>
            LeftUp = 0x0004,

            /// <summary>
            /// Specifies that the right button was pressed.
            /// </summary>
            RightDown = 0x0008,

            /// <summary>
            /// Specifies that the right button was released.
            /// </summary>
            RightUp = 0x0010,

            /// <summary>
            /// Specifies that the middle button was pressed.
            /// </summary>
            MiddleDown = 0x0020,

            /// <summary>
            /// Specifies that the middle button was released.
            /// </summary>
            MiddleUp = 0x0040,

            /// <summary>
            /// Windows 2000/XP: Specifies that an X button was pressed.
            /// </summary>
            XDown = 0x0080,

            /// <summary>
            /// Windows 2000/XP: Specifies that an X button was released.
            /// </summary>
            XUp = 0x0100,

            /// <summary>
            /// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
            /// </summary>
            VerticalWheel = 0x0800,

            /// <summary>
            /// Specifies that the wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. Windows 2000/XP:  Not supported.
            /// </summary>
            HorizontalWheel = 0x1000,

            /// <summary>
            /// Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
            /// </summary>
            VirtualDesk = 0x4000,

            /// <summary>
            /// Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
            /// </summary>
            Absolute = 0x8000,
        }

        public void CursorClick(MouseButtonsEnum mouseButtonEnum)
        {
            int x = System.Windows.Forms.Cursor.Position.X;
            int y = System.Windows.Forms.Cursor.Position.Y;

            Thread.Sleep(100);
            if (mouseButtonEnum == MouseButtonsEnum.RIGHT_BUTTON)
            {
                mouse_event(0x0008, x, y, 0, 0);
                Thread.Sleep(50);
                mouse_event(0x0010, x, y, 0, 0);
            }
            else if (mouseButtonEnum == MouseButtonsEnum.LEFT_BUTTON)
            {
                mouse_event(0x0002, x, y, 0, 0);
                Thread.Sleep(50);
                mouse_event(0x0004, x, y, 0, 0);
            }

        }


        public List<string> GetProcessWindowTitles()
        {
            return Process
                .GetProcesses()
                .Where(process => !String.IsNullOrEmpty(process.MainWindowTitle))
                .Select(x => x.MainWindowTitle)
                .ToList();
        }


        //public void GetDpi(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        //{
        //    var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
        //    var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
        //    GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        //}


        //private List<string> kek(DpiType type)
        //{
        //    var list = new List<string>();
        //    foreach (var screen in System.Windows.Forms.Screen.AllScreens)
        //    {
        //        uint x, y;
        //        GetDpi(screen, type, out x, out y);
        //        list.Add(screen.DeviceName + " - dpiX=" + x + ", dpiY=" + y);
        //    }
        //    return list;
        //}




        public async Task SaveImageToDisk(string filePath, byte[] image)
        {
            await File.WriteAllBytesAsync(filePath, image);
        }

        public void CreateFolderOnDisk(string folderName)
        {
            string folderUrl = PathHelper.GetAppDataPath() + "\\" + folderName;
            Directory.CreateDirectory(folderUrl);
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
            var applicationProcess = Process.GetProcessesByName(processName).FirstOrDefault();
            if (applicationProcess != null)
            {

                var applicationHandle = applicationProcess.MainWindowHandle.ToInt32();

                GetWindowRect(applicationHandle, out Model.Structs.Rectangle windowRectangle);

                return windowRectangle;
            }

            return new Model.Structs.Rectangle();
        }

        public bool MoveWindow(string processName, Model.Structs.Rectangle newWindowSize)
        {
            var applicationProcess = Process.GetProcessesByName(processName).FirstOrDefault();
            if (applicationProcess == null)
                return false;

            var applicationHandle = applicationProcess.MainWindowHandle.ToInt32();

            int x = newWindowSize.Left;
            int y = newWindowSize.Top;
            int width = newWindowSize.Right - newWindowSize.Left;
            int height = newWindowSize.Bottom - newWindowSize.Top;

            bool result = MoveWindow(applicationHandle, x, y, width, height, true);
            return result;
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

        public ImageSizeResult GetImageSize(byte[] imageArray)
        {
            ImageSizeResult imageSizeResult = new ImageSizeResult();
            Bitmap image;
            using (var ms = new MemoryStream(imageArray))
            {
                image = new Bitmap(ms);
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
            //var json = System.Text.Json.JsonSerializer.Serialize( flowsDto );

            string json = JsonConvert.SerializeObject(flowsDto, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });

            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "FlowsExport.json");
            await File.WriteAllTextAsync(filePath, json);

            return;
        }

        //public static object DeserializeFromStream(Stream stream)
        //{
        //    var serializer = new JsonSerializer();

        //    using (var sr = new StringWriter(stream))
        //    using (var jsonTextReader = new JsonTextWriter(sr))
        //    {
        //        return serializer.Serialize(jsonTextReader);
        //    }
        //}

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
            string filePath = Path.Combine(PathHelper.GetAppDataPath(), "FlowsExport.json");
            string flowsJSON = File.ReadAllText(filePath);
            if (flowsJSON != null)
            {
                var JsonFlows = JsonConvert.DeserializeObject<List<Flow>>(flowsJSON);
                if (JsonFlows != null && JsonFlows.Count > 0)
                    return JsonFlows;
            }

            return null;
        }


        //public void GetScalingFactor()
        //{
        //    List<double> physicalWidths = new List<double>();

        //    //Get physical width for each monitor
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");

        //    foreach (ManagementObject monitor in searcher.Get())
        //    {
        //        //Get the physical width (inch)
        //        double width = (byte)monitor["MaxHorizontalImageSize"] / 2.54;
        //        physicalWidths.Add(width);
        //    }

        //    //Get screen info for each monitor
        //    Screen[] screenList = Screen.AllScreens;
        //    int i = 0;

        //    foreach (Screen screen in screenList)
        //    {
        //        //Get the physical width (pixel)
        //        double physicalWidth;
        //        if (i < physicalWidths.Count)
        //        {
        //            //Get the DPI
        //            uint x, y;
        //            GetDpi2(screen, DpiType.Effective, out x, out y);

        //            //Convert inch to pixel
        //            physicalWidth = physicalWidths[i] * x;
        //        }
        //        else
        //        {
        //            physicalWidth = SystemParameters.PrimaryScreenWidth;
        //        }
        //        i++;

        //        //Calculate the scaling
        //        double scaling = 100 * (physicalWidth / screen.Bounds.Width);
        //        double scalingFactor = physicalWidth / screen.Bounds.Width;

        //        //Output the result
        //        Console.WriteLine(scalingFactor);
        //    }
        //}

        //public void GetDpi2(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        //{
        //    var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
        //    var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
        //    GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        //}

        //public List<double> GetScalingFactor2()
        //{
        //    List<double> physicalWidths = new List<double>();
        //    List<double> monitorsfactor = new List<double>();

        //    //Get physical width for each monitor
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");

        //    foreach (ManagementObject monitor in searcher.Get())
        //    {
        //        //Get the physical width (inch)
        //        double width = (byte)monitor["MaxHorizontalImageSize"] / 2.54;
        //        physicalWidths.Add(width);
        //    }

        //    //Get screen info for each monitor
        //    Screen[] screenList = Screen.AllScreens;
        //    int i = 0;

        //    foreach (Screen screen in screenList)
        //    {
        //        //Get the physical width (pixel)
        //        double physicalWidth;
        //        if (i < physicalWidths.Count)
        //        {
        //            //Get the DPI
        //            uint x, y;
        //            GetDpi3(screen, DpiType.Effective, out x, out y);

        //            //Convert inch to pixel
        //            physicalWidth = physicalWidths[i] * x;
        //        }
        //        else
        //        {
        //            physicalWidth = SystemParameters.PrimaryScreenWidth;
        //        }
        //        i++;

        //        //Calculate the scaling
        //        double scaling = 100 * (physicalWidth / screen.Bounds.Width);
        //        double scalingFactor = physicalWidth / screen.Bounds.Width;

        //        //Output the result
        //        Console.WriteLine(scalingFactor);
        //        monitorsfactor.Add(scalingFactor);
        //    }
        //    return monitorsfactor;
        //}

        //public void GetDpi3(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        //{
        //    var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
        //    var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
        //    GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        //}

    }
}
