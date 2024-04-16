using System.Runtime.InteropServices;

namespace Model.Structs
{
   [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
