using System.Diagnostics;
using FortiClientAutoLogin.AppSettings;
using FortiClientAutoLogin.Passwords;

namespace FortiClientAutoLogin.VpnLogger
{
    internal class AutoLogin
    {
        private readonly TokenReceiver tokenReceiver;

        public AutoLogin()
        {
            tokenReceiver = new TokenReceiver();
        }

        internal void Login()
        {
            var settings = Settings.Read();
            Process.Start(settings.Vpn.FortiClientExePath);
            var mainWindow = FindMainWindow();

            WindowInteract.Click(mainWindow, 100, 100);
            WindowInteract.SendKeys(mainWindow, "{TAB}{TAB}");
            WindowInteract.SendKeys(mainWindow, settings.Vpn.Login);
            WindowInteract.SendKeys(mainWindow, "{TAB}");
            WindowInteract.SendKeys(mainWindow, PasswordManager.Load(PasswordType.VPN));
            WindowInteract.SendKeys(mainWindow, "{ENTER}");

            var token = tokenReceiver.GetToken();
            WindowInteract.SendKeys(mainWindow, token);
            WindowInteract.SendKeys(mainWindow, "{ENTER}");

        }

        private static IntPtr FindMainWindow()
        {
            var mainWindow = IntPtr.Zero;
            while (mainWindow == IntPtr.Zero)
            {
                mainWindow = WindowInteract.GetWindowHandle("FortiClient -- The Security Fabric Agent");

                Application.DoEvents();
                Thread.Sleep(1000);
            }

            return mainWindow;
        }
    }
}
