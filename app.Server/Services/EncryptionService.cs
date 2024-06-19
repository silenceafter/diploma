using app.Server.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace app.Server.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConfiguration _configuration;

        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ComputeHash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(bytes);
            }
        }

        public string Encrypt(string value)
        {
            using (Aes aesAlg = Aes.Create())
            {
                //генерация ключа
                byte[] keyBytes = Convert.FromBase64String(_configuration["Encryption:Key"]);
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    //
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        swEncrypt.Write(value);
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Decrypt(string value)
        {
            // Преобразование зашифрованного текста обратно в массив байтов
            byte[] fullCipher = Convert.FromBase64String(value);
            using (Aes aesAlg = Aes.Create())
            {
                // Извлечение длины IV из первых 4 байтов зашифрованного текста
                int ivLength = BitConverter.ToInt32(fullCipher, 0);

                // Извлечение IV
                byte[] iv = new byte[ivLength];
                Array.Copy(fullCipher, sizeof(int), iv, 0, iv.Length);

                // Извлечение зашифрованного текста (без IV и длины IV)
                byte[] cipherBytes = new byte[fullCipher.Length - sizeof(int) - iv.Length];
                Array.Copy(fullCipher, sizeof(int) + iv.Length, cipherBytes, 0, cipherBytes.Length);

                // Установка ключа и IV для алгоритма AES
                byte[] keyBytes = Convert.FromBase64String(_configuration["Encryption:Key"]);
                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                // Создание дешифратора
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Дешифрование
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    // Возвращаем расшифрованный текст
                    return srDecrypt.ReadToEnd();
                }
            }
        } 
    }
}
