using System;
using System.Linq;
using System.Security.Cryptography;

namespace Api.FurnitureStore.Shared.Common
{
    public static class RandomGenerator
    {

        public static string GenerateRandomString(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
            }

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz$#-_.";
            var result = new char[size];
            var randomBytes = new byte[size];
            RandomNumberGenerator.Fill(randomBytes);

            for (var i = 0; i < size; i++)
            {
                var index = randomBytes[i] % chars.Length;
                result[i] = chars[index];
            }

            return new string(result);
        }
    }

}
