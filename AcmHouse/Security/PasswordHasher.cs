using System;
using System.Security.Cryptography;

namespace AcmHouse.Security
{
    /// <summary>
    /// Hashes passwords with PBKDF2 (HMAC-SHA256) and a random salt.
    /// Stored format: PBKDF2$iterations$saltBase64$hashBase64
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int DefaultIterations = 100000;

        public static string Hash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Derive(password, salt, DefaultIterations);
            return $"{Prefix}${DefaultIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (password == null || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('$');
            if (parts.Length != 4 || parts[0] != Prefix)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out int iterations) || iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual = Derive(password, salt, iterations, expected.Length);
            return FixedTimeEquals(actual, expected);
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int length = HashSize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(length);
            }
        }

        /// <summary>Compares in constant time so the check does not leak timing information.</summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}
