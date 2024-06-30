using Business.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Model.Models;
using Business.Interfaces;
using Model.Structs;
using Business.Helpers;
using Model.Business;
using Model.Enums;
using DataAccess.Repository.Interface;
using Force.DeepCloner;
using System.Windows.Navigation;
using Wpf.Ui.Controls;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows;
using System.Security;
using System.Management;
using System.Collections.ObjectModel;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FlowStepDetailTemplateSearchViewModel : ObservableObject, INavigationAware
    {
        private readonly ISystemService _systemService;
        private readonly ITemplateSearchService _templateMatchingService;
        private readonly IBaseDatawork _baseDatawork;
        private FlowsViewModel _flowsViewModel;

        [ObservableProperty]
        private List<string> _processList = SystemProcessHelper.GetProcessWindowTitles();

        [ObservableProperty]
        private string _templateImgPath = "";

        [ObservableProperty]
        private FlowStep _flowStep;

        public event ShowTemplateImgEvent? ShowTemplateImg;
        public delegate void ShowTemplateImgEvent(string filePath);

        public event ShowResultImageEvent? ShowResultImage;
        public delegate void ShowResultImageEvent(string filePath);

        public FlowStepDetailTemplateSearchViewModel(FlowStep flowStep, FlowsViewModel flowsViewModel, ISystemService systemService, ITemplateSearchService templateMatchingService, IBaseDatawork baseDatawork)
        {

            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _templateMatchingService = templateMatchingService;

            _flowStep = flowStep;
            _flowsViewModel = flowsViewModel;

            TemplateImgPath = flowStep.TemplateImagePath;
            ShowTemplateImg?.Invoke(TemplateImgPath);

        }

        [RelayCommand]
        private void OnButtonOpenFileClick()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = PathHelper.GetAppDataPath();
            openFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                TemplateImgPath = openFileDialog.FileName;
                ShowTemplateImg?.Invoke(openFileDialog.FileName);
            }

        }


        [RelayCommand]
        private void OnButtonTestClick()
        {
            Rectangle searchRectangle;

            if (FlowStep.ProcessName.Length > 0 && TemplateImgPath != null)
                searchRectangle = _systemService.GetWindowSize(FlowStep.ProcessName);
            else
                searchRectangle = _systemService.GetScreenSize();


            TemplateMatchingResult result = _templateMatchingService.SearchForTemplate(TemplateImgPath ?? "", searchRectangle);


            int x = searchRectangle.Left;
            int y = searchRectangle.Top;

            x = x + result.ResultRectangle.Top;
            y = y + result.ResultRectangle.Left;

            //_systemService.SetCursorPossition(x, y);


            ShowResultImage?.Invoke(result.ResultImagePath);
        }

        [RelayCommand]
        private void OnButtonCancelClick()
        {
            //TODO
        }

        [RelayCommand]
        private async Task OnButtonSaveClick()
        {
            if (FlowStep.ProcessName != null && TemplateImgPath != null)
            {
                FlowStep.TemplateImagePath = TemplateImgPath;

                // Edit mode
                if (FlowStep.Id > 0)
                {

                }

                /// Add mode
                else
                {

                    if (FlowStep.ParentFlowStepId != null)
                    {
                        FlowStep isNewSimpling = _baseDatawork.FlowSteps
                            .Where(x => x.Id == FlowStep.ParentFlowStepId)
                            .Select(x => x.ChildrenFlowSteps.First(y => y.FlowStepType == FlowStepTypesEnum.IS_NEW)).First();

                        FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                        isNewSimpling.OrderingNum++;

                    }
                    else
                    {
                        FlowStep isNewSimpling = _baseDatawork.Flows
                            .Where(x => x.Id == FlowStep.FlowId)
                            .Select(x => x.FlowSteps.First(y => y.FlowStepType== FlowStepTypesEnum.IS_NEW)).First();

                        FlowStep.OrderingNum = isNewSimpling.OrderingNum;
                        isNewSimpling.OrderingNum++;
                    }

                    // "Add" Flow steps
                    FlowStep newFlowStep = new FlowStep();
                    FlowStep newFlowStep2 = new FlowStep();
                    newFlowStep.FlowStepType= FlowStepTypesEnum.IS_NEW;
                    newFlowStep2.FlowStepType= FlowStepTypesEnum.IS_NEW;

                    // "Success" Flow step
                    FlowStep successFlowStep = new FlowStep();
                    successFlowStep.Name = "Success";
                    successFlowStep.IsExpanded = false;
                    successFlowStep.FlowStepType = FlowStepTypesEnum.IS_SUCCESS;
                    successFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        newFlowStep
                    };

                    // "Fail" Flow step
                    FlowStep failFlowStep = new FlowStep();
                    failFlowStep.Name = "Fail";
                    failFlowStep.IsExpanded = false;
                    failFlowStep.FlowStepType = FlowStepTypesEnum.IS_FAILURE;
                    failFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        newFlowStep2
                    };

                    FlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>
                    {
                        successFlowStep,
                        failFlowStep
                    };

                    //FlowStep.Flow = null;
                    _baseDatawork.FlowSteps.Add(FlowStep);
                }



                _baseDatawork.SaveChanges();
                await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());
                _flowsViewModel.RefreshData();
            }

            await _systemService.UpdateFlowsJSON(_baseDatawork.Flows.GetAll());

            //refreshtest?.Invoke();

        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szPhysicalMonitorDescription;
        }






        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        public static extern IntPtr MonitorFromWindow(
           [In] IntPtr hwnd, uint dwFlags);


        //[DllImport("dxva2.dll", EntryPoint = "GetMonitorTechnologyType")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetMonitorTechnologyType(
        //    IntPtr hMonitor, ref MC_DISPLAY_TECHNOLOGY_TYPE pdtyDisplayTechnologyType);



        [DllImport("dxva2.dll", EntryPoint = "GetMonitorCapabilities")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorCapabilities(
            IntPtr hMonitor, ref uint pdwMonitorCapabilities, ref uint pdwSupportedColorTemperatures);



        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(
            uint dwPhysicalMonitorArraySize, ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);



        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);



        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    }






    public static class DPIUtil
    {
        /// <summary>
        /// Min OS version build that supports DPI per monitor
        /// </summary>
        private const int MinOSVersionBuild = 14393;

        /// <summary>
        /// Min OS version major build that support DPI per monitor
        /// </summary>
        private const int MinOSVersionMajor = 10;

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        private static bool _isSupportingDpiPerMonitor;

        /// <summary>
        /// Flag, if OS version checked
        /// </summary>
        private static bool _isOSVersionChecked;

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        internal static bool IsSupportingDpiPerMonitor
        {
            get
            {
                if (_isOSVersionChecked)
                {
                    return _isSupportingDpiPerMonitor;
                }

                _isOSVersionChecked = true;
                var osVersionInfo = new OSVERSIONINFOEXW
                {
                    dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEXW))
                };

                if (RtlGetVersion(ref osVersionInfo) != 0)
                {
                    _isSupportingDpiPerMonitor = Environment.OSVersion.Version.Major >= MinOSVersionMajor && Environment.OSVersion.Version.Build >= MinOSVersionBuild;

                    return _isSupportingDpiPerMonitor;
                }

                _isSupportingDpiPerMonitor = osVersionInfo.dwMajorVersion >= MinOSVersionMajor && osVersionInfo.dwBuildNumber >= MinOSVersionBuild;

                return _isSupportingDpiPerMonitor;
            }
        }





        /// <summary>
        /// Get scale factor for an each monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> Scale factor </returns>
        public static double ScaleFactor(Control control, Model.Structs.Point monitorPoint)
        {


            var dpi = GetDpi(control, monitorPoint);

            return dpi * 100 / 96.0;
        }

        /// <summary>
        /// Get DPI for a monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> DPI </returns>
        public static uint GetDpi(Control control, Model.Structs.Point monitorPoint)
        {
            uint dpiX;
            uint dpiY;

            if (IsSupportingDpiPerMonitor)
            {
                var monitorFromPoint = MonitorFromPoint(monitorPoint, 2);

                GetDpiForMonitor(monitorFromPoint, DpiType.Effective, out dpiX, out dpiY);
            }
            else
            {
                // If using with System.Windows.Forms - can be used Control.DeviceDpi
                dpiX = control == null ? 96 : (uint)control.DeviceDpi;
            }

            return dpiX;
        }

        /// <summary>
        /// Retrieves a handle to the display monitor that contains a specified point.
        /// </summary>
        /// <param name="pt"> Specifies the point of interest in virtual-screen coordinates. </param>
        /// <param name="dwFlags"> Determines the function's return value if the point is not contained within any display monitor. </param>
        /// <returns> If the point is contained by a display monitor, the return value is an HMONITOR handle to that display monitor. </returns>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfrompoint"/>
        /// </remarks>
        [DllImport("User32.dll")]
        internal static extern IntPtr MonitorFromPoint([In] Model.Structs.Point pt, [In] uint dwFlags);

        /// <summary>
        /// Queries the dots per inch (dpi) of a display.
        /// </summary>
        /// <param name="hmonitor"> Handle of the monitor being queried. </param>
        /// <param name="dpiType"> The type of DPI being queried. </param>
        /// <param name="dpiX"> The value of the DPI along the X axis. </param>
        /// <param name="dpiY"> The value of the DPI along the Y axis. </param>
        /// <returns> Status success </returns>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/nf-shellscalingapi-getdpiformonitor"/>
        /// </remarks>
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        /// <summary>
        /// The RtlGetVersion routine returns version information about the currently running operating system.
        /// </summary>
        /// <param name="versionInfo"> Operating system version information </param>
        /// <returns> Status success</returns>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/wdm/nf-wdm-rtlgetversion"/>
        /// </remarks>
        [SecurityCritical]
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion(ref OSVERSIONINFOEXW versionInfo);

        /// <summary>
        /// Contains operating system version information.
        /// </summary>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-osversioninfoexw"/>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEXW
        {
            /// <summary>
            /// The size of this data structure, in bytes
            /// </summary>
            internal int dwOSVersionInfoSize;

            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            internal int dwMajorVersion;

            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            internal int dwMinorVersion;

            /// <summary>
            /// The build number of the operating system.
            /// </summary>
            internal int dwBuildNumber;

            /// <summary>
            /// The operating system platform.
            /// </summary>
            internal int dwPlatformId;

            /// <summary>
            /// A null-terminated string, such as "Service Pack 3", that indicates the latest Service Pack installed on the system.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string szCSDVersion;

            /// <summary>
            /// The major version number of the latest Service Pack installed on the system. 
            /// </summary>
            internal ushort wServicePackMajor;

            /// <summary>
            /// The minor version number of the latest Service Pack installed on the system.
            /// </summary>
            internal ushort wServicePackMinor;

            /// <summary>
            /// A bit mask that identifies the product suites available on the system. 
            /// </summary>
            internal short wSuiteMask;

            /// <summary>
            /// Any additional information about the system.
            /// </summary>
            internal byte wProductType;

            /// <summary>
            /// Reserved for future use.
            /// </summary>
            internal byte wReserved;
        }

        /// <summary>
        /// DPI type
        /// </summary>
        /// <remarks>
        /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/shellscalingapi/ne-shellscalingapi-monitor_dpi_type"/>
        /// </remarks>
        private enum DpiType
        {
            /// <summary>
            /// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements.
            /// </summary>
            Effective = 0,

            /// <summary>
            /// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen.
            /// </summary>
            Angular = 1,

            /// <summary>
            /// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself. Use this value when you want to read the pixel density and not the recommended scaling setting.
            /// </summary>
            Raw = 2,
        }










    }
}

