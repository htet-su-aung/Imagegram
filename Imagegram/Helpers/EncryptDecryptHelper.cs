using System;
using System.IO;
using System.Security.Cryptography;

namespace Imagegram.Helpers
{
    public static class EncryptDecryptHelper
    {
        public static string Encrypt(string text)
        {
            byte[] encrypted;
            byte[] key = System.Text.Encoding.UTF8.GetBytes(ConfigHelper.GetValue("EncKey"));
            using (AesManaged aes = new AesManaged())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(text);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);

        }

        public static string Decrypt(string textStr)
            {
                string plainText = null;
                byte[] text = Convert.FromBase64String(textStr);
            byte[] key = System.Text.Encoding.UTF8.GetBytes(ConfigHelper.GetValue("EncKey"));
            using (Aes aes = new AesManaged())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    ICryptoTransform decryptor = aes.CreateDecryptor();

                    using (MemoryStream ms = new MemoryStream(text))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                plainText = reader.ReadToEnd();
                            }
                        }
                    }
                }
                return plainText;
            }

        }
    }
