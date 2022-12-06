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
                while (token == null)
                {
                    var searchResult = client.Inbox.Search(searchQuery);
                    if (searchResult?.Any() == true)
                    {
                        var tokenMessage = client.Inbox.GetMessage(searchResult.First());
                        token = tokenMessage?.Subject.Replace("Token code:", "").Trim();

                        if (token != null)
                        {
                            client.Inbox.AddFlags(new UniqueId[] { searchResult.First() }, MessageFlags.Deleted, false);
                            client.Inbox.Expunge();
                        }
                    }
                    else
                    {
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
