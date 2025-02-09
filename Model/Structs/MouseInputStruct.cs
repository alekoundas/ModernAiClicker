using Model.Enums;
using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInputStruct
    {
        public int X;
        public int Y;
        public int MouseData;
        public MouseFlagEnum Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
}
