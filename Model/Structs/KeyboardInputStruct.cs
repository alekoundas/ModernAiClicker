using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInputStruct
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
}
