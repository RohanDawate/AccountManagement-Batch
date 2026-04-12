using System.Security.Cryptography;
using System.Text;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class LogEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public LogEncryptionService(string key, string iv)
        {
            // AES-256 requires 32 bytes. We'll ensure the key is exactly 32 bytes.
            _key = Encoding.UTF8.GetBytes(key).Take(32).ToArray();
            _iv = Convert.FromBase64String(iv);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (CryptographicException)
            {
                // If decryption fails (e.g. data wasn't encrypted), return original
                return cipherText;
            }
        }
    }
}
