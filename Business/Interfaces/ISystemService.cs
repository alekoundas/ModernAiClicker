
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
        Model.Structs.Rectangle GetScreenSize();
        bool MoveWindow(string processName, Model.Structs.Rectangle newWindowSize);
        void SetCursorPossition(Model.Structs.Point point);
        void CursorClick(MouseButtonsEnum mouseButtonEnum);
        Task UpdateFlowsJSON(List<Flow> flows);
        List<Flow>? LoadFlowsJSON();

        //List<double> GetScalingFactor2();
        ImageSizeResult GetImageSize(byte[] imagePath);

        List<string> GetProcessWindowTitles();
        void CreateFolderOnDisk(string folderName);
        Task SaveImageToDisk(string filePath, byte[] image);


        void CursorScroll(MouseScrollDirectionEnum scrollDirection, int steps);

    }
}
