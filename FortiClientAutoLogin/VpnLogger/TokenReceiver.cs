using FortiClientAutoLogin.AppSettings;
using FortiClientAutoLogin.Passwords;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
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

                var searchQuery = SearchQuery.DeliveredAfter(DateTime.Now.AddMinutes(-5))
                    .And(SearchQuery.SubjectContains("Token code:"));

                client.Inbox.Open(FolderAccess.ReadWrite);
                var index = (client.Inbox.Any() ? client.Inbox.Count() - 1 : 0);
                var startTime = DateTime.Now;
                while (token == null)
                {
                    if (DateTime.Now.Subtract(startTime).TotalSeconds > settings.TokenWaitingTimeoutInSec)
                        throw new TimeoutException("Token not found in inbox. Token waiting timeout.");

                    try
                    {
                        var mes = client.Inbox.GetMessage(index);

                        if (mes.Subject.Contains("Token code:"))
                        {
                            token = mes.Subject.Replace("Token code:", "").Trim();
                            client.Inbox.AddFlags(index, MessageFlags.Deleted, false);
                            client.Inbox.Expunge();
                            break;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Wait(client);
                    }
                }

                client.Inbox.Close();
                client.Disconnect(true);
            }

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
