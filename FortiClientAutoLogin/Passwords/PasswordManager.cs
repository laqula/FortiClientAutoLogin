using System.Security.Cryptography;
using System.Text;

namespace FortiClientAutoLogin.Passwords
{
    internal static class PasswordManager
    {
        public static void Save(PasswordType type, string password)
        {
            var passwordFilePath = $"{type}.bin";
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var protectedPasswordBytes = ProtectedData.Protect(passwordBytes, null, DataProtectionScope.CurrentUser);
            var protectedPasswordBytesBase64 = Convert.ToBase64String(protectedPasswordBytes);
            File.WriteAllText(passwordFilePath, protectedPasswordBytesBase64);
        }

        public static string Load(PasswordType type)
        {
            var passwordFilePath = $"{type}.bin";
            if (File.Exists(passwordFilePath))
            {
                var protectedPasswordBytesBase64 = File.ReadAllText(passwordFilePath);
                var protectedPasswordBytes = Convert.FromBase64String(protectedPasswordBytesBase64);
                var passwordBytes = ProtectedData.Unprotect(protectedPasswordBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(passwordBytes);
            }
            else
            {
                throw new FileNotFoundException("Password file not found. Save new password.");
            }
        }
    }
}
