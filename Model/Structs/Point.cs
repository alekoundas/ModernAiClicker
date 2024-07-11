using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;

namespace Model.Structs
{
   [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        [ObservableProperty]

        public int X;
        [ObservableProperty]

        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
