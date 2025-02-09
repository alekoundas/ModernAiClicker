using System.Runtime.InteropServices;

namespace Model.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MouseKeyboardHardwareInputStruct
    {
        [FieldOffset(0)]
        public HardwareInputStruct Hardware;

        [FieldOffset(0)]
        public KeyboardInputStruct Keyboard;

        [FieldOffset(0)]
        public MouseInputStruct Mouse;
    }
}
