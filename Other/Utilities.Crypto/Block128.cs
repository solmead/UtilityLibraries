using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Crypto
{
    public static class Block128
    {
        const int Pbkdf2Count = 1000;
        const int Pbkdf2SubkeyLength = (256 / 8);
        const int SaltSize = (256 / 8);
        const int IvSize = (128 / 8);
        const int Keysize = 256;
        const int DerivationIterations = 1000;


        public static string Encrypt(string plainText, string passPhrase)
        {
            //  Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            //  so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes((Keysize / 8));
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                //  Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            //  Get the complete stream of bytes that represent:
            //  [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            //  Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(SaltSize).ToArray();
            //  Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(SaltSize).Take(IvSize).ToArray();
            //  Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(SaltSize + IvSize).Take(
                (cipherTextBytesWithSaltAndIv.Length - (SaltSize + IvSize))).ToArray();
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes((Keysize / 8));
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        var memoryStream2 = new MemoryStream();
                        var cryptoStream2 = new CryptoStream(memoryStream2, decryptor, CryptoStreamMode.Write);
                        cryptoStream2.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                        cryptoStream2.FlushFinalBlock();
                        cryptoStream2.Close();
                        var answer = Encoding.UTF8.GetString(memoryStream2.ToArray());


                        return answer;


                        //using (var memoryStream = new MemoryStream(cipherTextBytes))
                        //{
                        //    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        //    {
                        //        var plainTextBytes = new byte[cipherTextBytes.Length];
                        //        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        //        cryptoStream.FlushFinalBlock();
                        //        memoryStream.Close();
                        //        cryptoStream.Close();
                        //        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        //    }
                        //}
                    }
                }
            }
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                //  Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                //  Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static string HashPassword(string password)
        {
            if ((password == null))
            {
                throw new ArgumentNullException("password");
            }

            byte[] salt;
            byte[] subkey;
            var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, Pbkdf2Count);
            salt = deriveBytes.Salt;
            subkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            byte[] outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, (1 + SaltSize), Pbkdf2SubkeyLength);
            var result = Convert.ToBase64String(outputBytes);
            if (!VerifyHashedPassword(result, password))
            {
                throw new Exception("Something wrong in crypto layer");
            }
            return result;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            try
            {
                if ((hashedPassword == null))
                {
                    throw new ArgumentNullException("hashedPassword needed");
                }

                if ((password == null))
                {
                    throw new ArgumentNullException("password needed");
                }

                byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
                // // Verify a version 0 (see comment above) password hash.
                if (((hashedPasswordBytes.Length != (1
                            + (SaltSize + Pbkdf2SubkeyLength)))
                            || (hashedPasswordBytes[0] != 0)))
                {
                    // // Wrong length or version header.
                    return false;
                }

                byte[] salt = new byte[SaltSize];
                Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
                byte[] storedSubkey = new byte[Pbkdf2SubkeyLength];
                Buffer.BlockCopy(hashedPasswordBytes, (1 + SaltSize), storedSubkey, 0, Pbkdf2SubkeyLength);
                byte[] generatedSubkey;
                var deriveBytes = new Rfc2898DeriveBytes(password, salt, Pbkdf2Count);
                generatedSubkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
                return ByteArraysEqual(storedSubkey, generatedSubkey);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame = areSame && (a[i] == b[i]);
            }
            return areSame;


        }
    }
}
