using FortiClientAutoLogin.VpnLogger;

namespace FortiClientAutoLogin
{
    public class Daemon : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly AutoLogin autoLogin;

        public Daemon()
        {
            autoLogin = new AutoLogin();
            trayIcon = CreateTrayIcon();
            trayIcon.Text = "FortiClient Auto Logger";
        }

        private NotifyIcon CreateTrayIcon()
        {
            var icon =  new NotifyIcon()
            {
                Icon = new Icon("icon.ico"),
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items =
                    {
                        new ToolStripMenuItem("Connect", null, new EventHandler(Connect), "CONNECT"),
                        new ToolStripMenuItem("Settings", null, new EventHandler(Settings), "SETTINGS"),
                        new ToolStripMenuItem("Set VPN password", null, new EventHandler(SetVpnPass), "VPN_PASS"),
                        new ToolStripMenuItem("Set mail password", null, new EventHandler(SetEmailPass), "MAIL_PASS"),
                        new ToolStripMenuItem("Close", null, new EventHandler(Exit), "EXIT")
                    }
                },
                Visible = true,
            };
            icon.MouseClick += Icon_MouseClick;

            return icon;
        }

        private void SetEmailPass(object? sender, EventArgs e)
        {
            new FrmMailPass().ShowDialog();
        }

        private void SetVpnPass(object? sender, EventArgs e)
        {
            new FrmVpnPass().ShowDialog();
        }

        private void Connect()
        {
            try
            {
                autoLogin.Login();
            }
            catch (Exception ex)
            {
                trayIcon.ShowBalloonTip(5000, "Error", ex.Message, ToolTipIcon.Error);
            }
        }

        private void Icon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Connect();
            }
        }

        private void Connect(object? sender, EventArgs e) => Connect();

        private void Exit(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Settings(object? sender, EventArgs e)
        {
            AppSettings.Settings.OpenFileInEditor();
        }
    }
}
