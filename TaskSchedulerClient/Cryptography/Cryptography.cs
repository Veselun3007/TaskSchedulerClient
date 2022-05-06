using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.CryptographyMethods
{
    public class Cryptography
    {

        #region *** Fields + Сonstructor ***

        private readonly IConfiguration _configuration;

        public Cryptography(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region *** KeyGenerate ***

        public void RSA_KeyGenerate()
        {
            using RSACryptoServiceProvider rsa = new(512);
            _configuration["PublicKey"] = rsa.ToXmlString(false);
            _configuration["Private_public_Key"] = rsa.ToXmlString(true);

        }
        #endregion

        #region *** Encrypt ***

        private static byte[] RSA_EncryptStringToBytes(byte[] decrypted, string key)
        {
            byte[] encrypted;
            using (RSACryptoServiceProvider rsa = new())
            {
                rsa.FromXmlString(key);
                encrypted = rsa.Encrypt(decrypted, false);
            }
            return encrypted;
        }

        private static string RSA_Encrypt(string plainText, string key)
        {
            return Convert.ToBase64String(RSA_EncryptStringToBytes(Encoding.UTF8.
                GetBytes(plainText), key));
        }

        /// <summary>
        /// Шифрує пароль об'єкта user, що наслідує інтерфейс IUser
        /// 
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, 
        /// пароль якого шифрується</param>
        /// <param name="key"></param>
        public object RSAEncryptIUser(IUser user, string key)
        {
            user.UserPassword = RSA_Encrypt(user.UserPassword, _configuration["APIkey"]);
            return user;
        }
        #endregion

        #region *** Decrypt ***
        private static byte[] RSA_DecryptStringToBytes(byte[] encrypted, string key)
        {
            byte[] decrypted;
            using (RSACryptoServiceProvider rsa = new())
            {
                rsa.FromXmlString(key);
                decrypted = rsa.Decrypt(encrypted, false);
            }
            return decrypted;
        }

        private static string RSA_Decrypt(string cypher, string key)
        {
            return Encoding.UTF8.GetString(RSA_DecryptStringToBytes(Convert.
                FromBase64String(cypher), key));
        }

        /// <summary>
        /// Дешифрує пароль об'єкта user, що був зашифрований відкритим ключом наданим API, 
        /// використовуючи закритий ключ API
        /// </summary>
        /// <param name="user">Об'єкт, що наслідує інтерфейс IUser, 
        /// пароль якого потрібно дешифрувати</param>
        public void RSA_Decrypt_IUser(IUser user)
        {
            user.UserPassword = RSA_Decrypt(user.UserPassword,
                _configuration["Private_public_Key"]);
        }
        #endregion

    }
}
