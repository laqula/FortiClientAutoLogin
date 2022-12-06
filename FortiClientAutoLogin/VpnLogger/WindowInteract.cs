using System.Runtime.InteropServices;

namespace FortiClientAutoLogin.VpnLogger
{
    internal static class WindowInteract
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);


        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_RBUTTONDOWN = 0x0204;
        private const uint WM_RBUTTONUP = 0x0205;

        private static int MAKELPARAM(int p, int p_2)
        {
            return ((p_2 << 16) | (p & 0xFFFF));
        }

        public static void Click(IntPtr handle, int x, int y)
        {
            SetForegroundWindow(handle);

            SendMessage(handle.ToInt32(), WM_LBUTTONDOWN, 0, MAKELPARAM(x, y));
            SendMessage(handle.ToInt32(), WM_LBUTTONUP, 0, MAKELPARAM(x, y));
        }

        public static void SendKeys(IntPtr handle, string keys)
        {
            SetForegroundWindow(handle);
            System.Windows.Forms.SendKeys.SendWait(keys);
        }

        public static IntPtr GetWindowHandle(string title)
        {
            return FindWindow(null, title);
        }
    }
}
