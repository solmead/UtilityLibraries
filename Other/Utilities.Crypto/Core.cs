using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Crypto
{
    public static class Core
    {


        public static string Encrypt(string plainText, string passPhrase)
        {
            try
            {
                var data = Block128.Encrypt(plainText, passPhrase);
                return data;
            } catch
            {
                //var data = Block256.Encrypt(plainText, passPhrase);
                return null;

            } 
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {

            try
            {
                var data = Block128.Decrypt(cipherText, passPhrase);
                return data;
            }
            catch
            {
                //var data = Block256.Decrypt(cipherText, passPhrase);
                return null;

            }
        }


        public static string HashPassword(string password)
        {
            return Block128.HashPassword(password);
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {

            return Block128.VerifyHashedPassword(hashedPassword, password);
        }

    }
}
