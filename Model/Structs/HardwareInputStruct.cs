using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInputStruct
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }
}
