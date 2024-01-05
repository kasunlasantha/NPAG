using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NPAG.Services
{
    public class DecryptorClass
    {
        public string Decrypt(String cipherText)
        {
            Byte[] initVectorBytes;
            Byte[] saltValueBytes;
            Byte[] cipherTextBytes;
            PasswordDeriveBytes password;
            Byte[] keyBytes;
            RijndaelManaged key;
            ICryptoTransform decryptor;
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Byte[] plainTextBytes = null;
            int decryptedByteCount;

            String passPhrase = "Status#1";
            String saltValue = "Ep1kS@lt";
            String hashAlgorithm = "SHA512"; //SHA256       //'You can use either MD5 or SHA1           
            int passwordIterations = 2;
            String initVector = "@1G2c7A4r5T6H7J4"; //'@1B2c3D4e5F6g7H8
            int keySize = 256;

            try
            {

                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                cipherTextBytes = Convert.FromBase64String(cipherText);


                password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                keyBytes = password.GetBytes(keySize / 8);
                key = new RijndaelManaged();
                key.Mode = CipherMode.CBC;
                decryptor = key.CreateDecryptor(keyBytes, initVectorBytes);


                memoryStream = new MemoryStream(cipherTextBytes);
                cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                //plainTextBytes.(cipherTextBytes.Length);
                // ReDim plainTextBytes(cipherTextBytes.Length)
                //plainTextBytes.Length = cipherTextBytes.Length;
                Array.Resize(ref plainTextBytes, cipherTextBytes.Length);
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);


                memoryStream.Close();
                cryptoStream.Close();
                string planText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                return planText;

            }
            catch (Exception)
            {
                throw new AuthenticationException("Error-6010: Invalid Security Code");
            }

        }
    }
}
