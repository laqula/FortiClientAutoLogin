using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace FortiClientAutoLogin.AppSettings
{
    internal class Settings
    {
        public int TokenWaitingTimeoutInSec { get; set; }
        public VpnSettings Vpn { get; set; }
        public MailSettings Mail { get; set; }

        public class VpnSettings
        {
            public string FortiClientExeDirPath { get; set; }
            public string FortiClientExePath { get; set; }
            public string Login { get; set; }
        }

        public class MailSettings
        {
            public string ImapLogin { get; set; }
            public string ImapServer { get; set; }
            public int ImapPort { get; set; }
            public string ImapSslOption { get; set; }
        }

        internal static Settings Read()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            return config.Get<Settings>();
        }

        internal static void OpenFileInEditor()
        {
            var filePath = Path.Combine(Application.StartupPath, "appsettings.json");
            var process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(Environment.SystemDirectory, "rundll32.exe"),
                Arguments = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll") + ",OpenAs_RunDLL " + filePath,
            };

            process.Start();
        }
    }
}
