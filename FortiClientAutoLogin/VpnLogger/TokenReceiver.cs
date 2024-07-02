using FortiClientAutoLogin.AppSettings;
using FortiClientAutoLogin.Passwords;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

namespace FortiClientAutoLogin.VpnLogger
{
    internal class TokenReceiver
    {
        public string GetToken()
        {
            var settings = Settings.Read();
            string? token = null;

            using (var client = new ImapClient())
            {
                client.Connect(
                    settings.Mail.ImapServer,
                    settings.Mail.ImapPort,
                    Enum.Parse<SecureSocketOptions>(settings.Mail.ImapSslOption));
                client.Authenticate(settings.Mail.ImapLogin, PasswordManager.Load(PasswordType.EMAIL));

                var nextUidInbox = GetNextUid(client.Inbox);
                var nextUidTrash = GetNextUid(client.GetFolder(SpecialFolder.Trash));
                var nextUidJunk = GetNextUid(client.GetFolder(SpecialFolder.Junk));

                var startTime = DateTime.Now;
                while (token == null)
                {
                    if (DateTime.Now.Subtract(startTime).TotalSeconds > settings.TokenWaitingTimeoutInSec)
                        throw new TimeoutException("Token not found in inbox. Token waiting timeout.");

                    token = CheckFolder(client.Inbox, nextUidInbox, token);
                    token = CheckFolder(client.GetFolder(SpecialFolder.Trash), nextUidTrash, token);
                    token = CheckFolder(client.GetFolder(SpecialFolder.Junk), nextUidJunk, token);

                    if (token == null)
                    {
                        Wait(client);
                    }
                }

                client.Disconnect(true);
            }

            return token;
        }

        private UniqueId GetNextUid(IMailFolder folder)
        {
            folder.Open(FolderAccess.ReadWrite);
            var nextUid = folder.UidNext.Value;
            folder.Close();
            return nextUid;
        }

        private string CheckFolder(IMailFolder folder, UniqueId nextUid, string token)
        {
            try
            {
                folder.Open(FolderAccess.ReadWrite);
                var mes = folder.GetMessage(nextUid);

                if (token == null && mes.Subject.Contains("Token code:"))
                {
                    token = mes.Subject.Replace("Token code:", "").Trim();
                    folder.AddFlags(nextUid, MessageFlags.Deleted, false);
                    folder.Expunge();
                }
            }
            catch (MessageNotFoundException) { }

            folder.Close();
            return token;
        }

        private static void Wait(ImapClient client)
        {
            client.NoOp();
            Application.DoEvents();
            Thread.Sleep(2000);
        }
    }
}
