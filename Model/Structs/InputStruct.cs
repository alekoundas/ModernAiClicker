using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InputStruct
    {
        public uint Type;
        public MouseKeyboardHardwareInputStruct Data;
    }
}
