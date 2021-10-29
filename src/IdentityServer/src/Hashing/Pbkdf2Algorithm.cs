using System;
using System.Security.Cryptography;

namespace IdOps.IdentityServer.Hashing
{
    internal class Pbkdf2Algorithm
    {
        // The following constants may be changed without breaking existing hashes.
        private static readonly int SALT_BYTE_SIZE = 24;
        private static readonly int HASH_BYTE_SIZE = 24;
        private static readonly int PBKDF2_ITERATIONS = 1000;
        private static readonly int ITERATION_INDEX = 0;
        private static readonly int SALT_INDEX = 1;
        private static readonly int PBKDF2_INDEX = 2;

        /// <summary>
        /// Creates a salted PBKDF2 hash of the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash of the password.</returns>
        public static string CreatePasswordHash(string password)
        {
            // Generate a random salt
            RandomNumberGenerator csprng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            // Hash the password and encode the parameters
            byte[] hash = GetPbkdf2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" +
                Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Validates a password given a hash of the correct one.
        /// </summary>
        /// <param name="password">The hashed password to check.</param>
        /// <param name="correctHash">A of the correct password.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public static bool ValidatePasswordHash(string password, string correctHash)
        {
            // Extract the parameters from the hash
            char[] delimiter =
            {
                ':'
            };
            string[] split = correctHash.Split(delimiter);
            int iterations = int.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
            byte[] testHash = GetPbkdf2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] GetPbkdf2(
            string password,
            byte[] salt,
            int iterations,
            int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
