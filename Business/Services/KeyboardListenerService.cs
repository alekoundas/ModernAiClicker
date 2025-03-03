using Business.Services.Interfaces;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Business.Services
{
    public class KeyboardListenerService : IKeyboardListenerService
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13; // Low-level keyboard hook
        private const int WM_KEYDOWN = 0x0100; // Key down event
        private static IntPtr _hookID = IntPtr.Zero;
        private readonly LowLevelKeyboardProc _proc;
        private readonly Dictionary<KeyCombination, Action> _listeners = new Dictionary<KeyCombination, Action>();

        public KeyboardListenerService()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        public void RegisterListener(KeyCombination combination, Action action)
        {
            _listeners[combination] = action;
        }

        public void UnregisterListener(KeyCombination combination)
        {
            _listeners.Remove(combination);
        }

        public void UnregisterAllListeners()
        {
            foreach (var listener in _listeners)
                _listeners.Remove(listener.Key);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                foreach (var listener in _listeners)
                {
                    if (listener.Key.IsMatch(key))
                    {
                        listener.Value.Invoke();
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


    }

    public class KeyCombination
    {
        public ModifierKeys Modifiers { get; }
        public Key Key { get; }

        public KeyCombination(ModifierKeys modifiers, Key key)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public bool IsMatch(Key pressedKey)
        {
            return pressedKey == Key && Keyboard.Modifiers == Modifiers;
            //return pressedKey == Key && (Keyboard.Modifiers & Modifiers) == Modifiers;
        }
    }
}
