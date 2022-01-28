using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Shared.DTOs;
using System.Security.Cryptography;

namespace ASP.NET_API.Services
{
    public class HashService
    {
        public HashResult Hash(string plainText)
        {
            var salt = new byte[16];

            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);

            return Hash(plainText, salt);
        }

        public HashResult Hash(string plainText, byte[] salt)
        {
            var derivedKey = KeyDerivation.Pbkdf2(password: plainText,
                salt: salt, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashResult
            {
                Hash = hash,
                Salt = salt,
            };
        }
    }
}
