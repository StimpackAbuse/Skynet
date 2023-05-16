using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Skynet
{
    public class Encryption
    {
        private static readonly int iteration = 100000;
        private static readonly string randomVector = "Xx1O4WBoJTUFCcECE3Mof2eG0vmtBjgv";

        public static Rfc2898DeriveBytes CreateKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);

            return new Rfc2898DeriveBytes(keyBytes, saltBytes, iteration);
        }

        public static Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);
            byte[] saltBytes = SHA512.Create().ComputeHash(vectorBytes);

            return new Rfc2898DeriveBytes(vectorBytes, saltBytes, iteration);
        }

        public static byte[] Encrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();
            Rfc2898DeriveBytes key = CreateKey(password);
            Rfc2898DeriveBytes vector = CreateVector(randomVector);

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);
            aes.IV = vector.GetBytes(16);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();
            Rfc2898DeriveBytes key = CreateKey(password);
            Rfc2898DeriveBytes vector = CreateVector(randomVector);

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);
            aes.IV = vector.GetBytes(16);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
