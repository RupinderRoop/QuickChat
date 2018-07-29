using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication
{
    class Crypto
    {
        public static string Encrypt(string orignal,string key)
        {
            using (MD5CryptoServiceProvider mdHash = new MD5CryptoServiceProvider())
            {
                byte[] text = UTF8Encoding.UTF8.GetBytes(orignal);

                byte[] keyArray = mdHash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                tds.Key = keyArray;
                tds.Mode = CipherMode.ECB;
                tds.Padding = PaddingMode.PKCS7;

                ICryptoTransform transform = tds.CreateEncryptor();

                byte[] result = transform.TransformFinalBlock(text, 0, text.Length);

                string endResult = Convert.ToBase64String(result);
                return endResult.ToString();
            }
                
        }

        public static string Decrypt(string encText, string key)
        {
            using (MD5CryptoServiceProvider mdhash = new MD5CryptoServiceProvider())
            {
                encText = encText.Replace("\0",null);
                byte[] encBytes = Convert.FromBase64String(encText);

                byte[] keyArray = mdhash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                tds.Key = keyArray;
                tds.Mode = CipherMode.ECB;
                tds.Padding = PaddingMode.PKCS7;

                ICryptoTransform transform = tds.CreateDecryptor();
                byte[] result = transform.TransformFinalBlock(encBytes, 0, encBytes.Length);
                string orignal = UTF8Encoding.UTF8.GetString(result);
                return orignal;
            }
        }
    }
}
