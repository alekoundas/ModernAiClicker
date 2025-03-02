using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowSize
    {
        //X
        public double Left; 

        //Y
        public double Top;

        public double Width;
        public double Height;
        public bool IsMaximized;
    }
}
