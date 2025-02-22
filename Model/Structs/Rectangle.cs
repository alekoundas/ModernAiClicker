using System.Runtime.InteropServices;

namespace Model.Structs
{
   [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left; //X
        public int Top; //Y
        public int Right;
        public int Bottom;
    }
}
