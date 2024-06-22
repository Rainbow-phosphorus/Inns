using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inns
{
    public class DataProtection
    {
        //вектор шифрования
        private static byte[] iv = { 3, 0, 6, 0, 7, 3, 0, 4, 10, 0, 1, 5, 0, 0, 2, 7 };

        //ключь шифрования
        private static string key = "password73590275";

        //метод шифрования данных
        public string Encrypt(string text)
        {
            byte[] Key = Encoding.UTF8.GetBytes(key);
                
            AesManaged aes = new AesManaged();
            aes.Key = Key;
            aes.IV = iv;

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] InputBytes = Encoding.UTF8.GetBytes(text);
            cryptoStream.Write(InputBytes, 0, InputBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] Encrypted = memoryStream.ToArray();
              
            return Convert.ToBase64String(Encrypted);
        }

        //метод расшифрования данных
        public string Decrypt(string text)
        {
            byte[] Key = Encoding.UTF8.GetBytes(key);

               
            AesManaged aes = new AesManaged();
            aes.Key = Key;
            aes.IV = iv;

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] InputBytes = Convert.FromBase64String(text);
            cryptoStream.Write(InputBytes, 0, InputBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] Decrypted = memoryStream.ToArray();
   
            return UTF8Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);
        }

        //метод хеширования данных
        public static byte[] PBKDF2Hash(string input)
        {
            byte[] salt = new byte[20];
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(input, salt, iterations: 5000);
            return pbkdf2.GetBytes(20);
        }
    }
}
