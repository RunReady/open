


using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Utility.Crypto
{
    /// <summary>
    /// Miscelaneous methods to simplify handling random generation for non-security people.
    /// </summary>
    public static class RandomNumberHelper
    {
        static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generates a cryptographically random array of bytes of the desired length in bytes.
        /// </summary>
        /// <param name="numberOfBytes"></param>
        /// <returns></returns>
        public static byte[] GenerateRandomBytes(int numberOfBytes)
        {
            var bytes = new byte[numberOfBytes];
            _random.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Creates a cryptographically random base64 encoded string
        /// </summary>
        /// <param name="numberOfChars">must be between 4 and 1024 inclusive and dividable by 4 to avaoid padding issues causing a bias</param>
        /// <returns></returns>
        public static string GetRandomBase64String(int numberOfChars)
        {
            if (numberOfChars < 4 || numberOfChars > 1024 || (numberOfChars % 4) != 0)
                throw new ArgumentOutOfRangeException(string.Format("numberofChars is {0}: It must be between 4 and 1024 inclusive and dividable by 4.", numberOfChars));
            int size = numberOfChars / 4 * 3;
            var bytes = GenerateRandomBytes(size);
            var randomBase64 = Convert.ToBase64String(bytes);
            return randomBase64;
        }

        /// <summary>
        /// Generates a cryptographically random number between two 
        /// values (inclusive of the values). maxValue must be greater 
        /// than minValue. The function has a bias if the span 
        /// (maxvalue - minValue + 1) is not a power of 2 number. The
        /// extend of the bias depends on the span versus the span of
        /// the 64 bit unisgned int. Below are helpers to return other 
        /// unsigned integer types
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static ulong GenerateRandomNumber(ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            if (maxValue <= minValue)
                throw new ArgumentException(string.Format("maxValue {0} must be greater than minValue {1}", maxValue, minValue));
            var size = sizeof(ulong);
            var bytes = GenerateRandomBytes(size);
            var unsigned = BitConverter.ToUInt64(bytes, 0);
            ulong result;
            if (minValue == ulong.MinValue && maxValue == ulong.MaxValue)
            {
                result = unsigned;
            }
            else
            {
                var span = maxValue - minValue + 1;
                var modulo = unsigned % span;
                result = modulo + minValue;
            }
            return result;
        }

        public static uint GenerateRandomNumber(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            return (uint)GenerateRandomNumber((ulong)minValue, (ulong)maxValue);
        }

        public static ushort GenerateRandomNumber(ushort minValue = ushort.MinValue, ushort maxValue = ushort.MaxValue)
        {
            return (ushort)GenerateRandomNumber((ulong)minValue, (ulong)maxValue);
        }

        public static byte GenerateRandomNumber(byte minValue = byte.MinValue, byte maxValue = byte.MaxValue)
        {
            return (byte)GenerateRandomNumber((ulong)minValue, (ulong)maxValue);
        }


        /// <summary>
        ///  Translates the unsigned function above to signed
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static long GenerateRandomNumber(long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            // check here to preserve arithmetic below since it could cause overflow issues
            if (maxValue <= minValue)
                throw new ArgumentException(string.Format("maxValue {0} must be greater than minValue {1}", maxValue, minValue));

            // shift the span to start at unsigned 0L for both positive and negative minValue
            var bigIntegerMinValue = (BigInteger)minValue;
            var bigIntegerMaxValue = (BigInteger)maxValue;
            var altMaxValue = (ulong)(bigIntegerMaxValue - bigIntegerMinValue);
            var altMinValue = 0UL;

            // use the unsigned version to get the value
            var unsignedResult = GenerateRandomNumber(altMinValue, altMaxValue);
            var unsignedBigIntegerResult = (BigInteger)unsignedResult;

            //shift the span back to the requested range for both positive and negative minValue
            var signedBigIntegerResult = unsignedBigIntegerResult + bigIntegerMinValue;
            var signedResult = (long)signedBigIntegerResult;

            // return the final result
            return signedResult;
        }



        public static int GenerateRandomNumber(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return (int)GenerateRandomNumber((long)minValue, (long)maxValue);
        }

        public static short GenerateRandomNumber(short minValue = short.MinValue, short maxValue = short.MaxValue)
        {
            return (short)GenerateRandomNumber((long)minValue, (long)maxValue);
        }

        public static sbyte GenerateRandomNumber(sbyte minValue = sbyte.MinValue, sbyte maxValue = sbyte.MaxValue)
        {
            return (sbyte)GenerateRandomNumber((long)minValue, (long)maxValue);
        }

    }
}
