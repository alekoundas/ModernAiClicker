﻿
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;

namespace Business.Interfaces
{
    public interface ISystemService
    {
        Bitmap? TakeScreenShot(Model.Structs.Rectangle rectangle, string filename = "Screenshot");

        Model.Structs.Rectangle GetWindowSize(string processName);
        bool MoveWindow(string processName, Model.Structs.Rectangle newWindowSize);
        Model.Structs.Rectangle GetScreenSize();
        void SetCursorPossition(Model.Structs.Point point);
        void CursorClick(MouseButtonsEnum mouseButtonEnum);
        Task UpdateFlowsJSON(List<Flow> flows);
        List<Flow>? LoadFlowsJSON();

        List<double> GetScalingFactor2();
        ImageSizeResult GetImageSize(string imagePath);

        List<string> GetProcessWindowTitles();
    }
}
