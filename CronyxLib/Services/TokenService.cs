using System.Security.Cryptography;
using System.Text;
namespace CronyxLib.Services
{
    public static class TokenService
    {
        private static string GetPath()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "alium"
            );

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, "token.dat");
        }

        public static bool SaveToken(string token)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(token);
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
                File.WriteAllBytes(GetPath(), encrypted);
                return true;
            }
            catch { return false; }
        }

        public static string? LoadToken()
        {
            var path = GetPath();

            if (!File.Exists(path))
                return null;

            try
            {
                byte[] encrypted = File.ReadAllBytes(path);
                byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch { return null; }
        }
    }
}