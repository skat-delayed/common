using System;
using System.Security.Cryptography;

namespace doe.Common.Security.Cryptography
{
    public class Common
    {
        /// <summary>
        /// Generates random integer.
        /// </summary>
        /// <param name="minValue">
        /// Min value (inclusive).
        /// </param>
        /// <param name="maxValue">
        /// Max value (inclusive).
        /// </param>
        /// <returns>
        /// Random integer value between the min and max values (inclusive).
        /// </returns>
        /// <remarks>
        /// This methods overcomes the limitations of .NET Framework's Random
        /// class, which - when initialized multiple times within a very short
        /// period of time - can generate the same "random" number.
        /// </remarks>
        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            var randomBytes = new byte[4];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);    
            }

            // Convert four random bytes into a positive integer value.
            var seed = ((randomBytes[0] & 0x7f) << 24) |
                       (randomBytes[1] << 16) |
                       (randomBytes[2] << 8) |
                       (randomBytes[3]);

            var random = new Random(seed);

            return random.Next(minValue, maxValue + 1);
        }

        /// <summary>
        /// Generates an array holding cryptographically strong bytes.
        /// </summary>
        /// <param name="saltLength">Length of the salt.</param>
        /// <returns>
        /// Array of randomly generated bytes.
        /// </returns>
        /// <remarks>
        /// The first four bytes of the salt array will contain the salt length
        /// split into four two-bit pieces.
        /// </remarks>
        public static byte[] GenerateSalt(int saltLength)
        {
            var salt = new byte[saltLength];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(salt);    
            }
            
            // Split salt length (always one byte) into four two-bit pieces and
            // store these pieces in the first four bytes of the salt array.
            salt[0] = (byte)((salt[0] & 0xfc) | (saltLength & 0x03));
            salt[1] = (byte)((salt[1] & 0xf3) | (saltLength & 0x0c));
            salt[2] = (byte)((salt[2] & 0xcf) | (saltLength & 0x30));
            salt[3] = (byte)((salt[3] & 0x3f) | (saltLength & 0xc0));

            return salt;
        }

        /// <summary>
        /// Adds an array of randomly generated bytes at the beginning of the
        /// array holding original plain text value.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Byte array containing original plain text value.
        /// </param>
        /// <param name="saltLength"></param>
        /// <returns>
        /// a modified array containing a randomly generated salt added at the 
        /// beginning of the plain text bytes. 
        /// </returns>
        public static byte[] AddSaltToBytes(byte[] plainTextBytes, int saltLength)
        {
            return AddSaltToBytes(plainTextBytes, GenerateSalt(saltLength));
        }

        /// <summary>
        /// Adds an array of randomly generated bytes at the beginning of the
        /// array holding original plain text value.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Byte array containing original plain text value.
        /// </param>
        /// <param name="saltBytes">
        /// Byte array containing salt value.
        /// </param>
        /// <returns>
        /// a modified array containing salt added at the beginning of the 
        /// plain text bytes. 
        /// </returns>
        public static byte[] AddSaltToBytes(byte[] plainTextBytes,
                                            byte[] saltBytes)
        {
            var plainTextBytesWithSalt = new byte[plainTextBytes.Length +
                                                  saltBytes.Length];
            
            Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);

            Array.Copy(plainTextBytes, 0,
                plainTextBytesWithSalt, saltBytes.Length,
                plainTextBytes.Length);

            return plainTextBytesWithSalt;
        }
    }
}