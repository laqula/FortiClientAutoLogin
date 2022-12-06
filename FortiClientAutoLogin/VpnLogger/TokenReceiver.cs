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
                var nextUid = client.Inbox.UidNext.Value.Id;
                while (token == null)
                {
                    try
                    {
                        var mes = client.Inbox.GetMessage(new UniqueId(nextUid));

                        if (mes.Subject.Contains("Token code:"))
                        {
                            token = mes.Subject.Replace("Token code:", "").Trim();
                            client.Inbox.AddFlags(new UniqueId(nextUid), MessageFlags.Deleted, false);
                            client.Inbox.Expunge();
                            break;
                        }
                        else
                        {
                            nextUid++;
                        }
                    }
                    catch (MessageNotFoundException)
                    {
                        client.NoOp();
                        Application.DoEvents();
                        Thread.Sleep(2000);
                    }
                }

                client.Inbox.Close();
                client.Disconnect(true);
            }

            return token;
        }
    }
}
