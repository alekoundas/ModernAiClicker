
using Model.Models;
using Model.Structs;
using System.Drawing;

namespace Business.Interfaces
{
    public interface ISystemService
    {
        Bitmap? TakeScreenShot(Model.Structs.Rectangle rectangle, string filename = "Screenshot");

        Model.Structs.Rectangle GetWindowSize(string processName);
        Model.Structs.Rectangle GetScreenSize();
        void SetCursorPossition(int x, int y);
        Task UpdateFlowsJSON(List<Flow> flows);
        List<Flow>? LoadFlowsJSON();

        List<double> GetScalingFactor2();
    }
}
