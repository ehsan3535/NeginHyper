using System;
using System.Linq;

namespace Common.Utilities
{
    public static class OtpGenerator
    {
        public static string GenerateRandomToken(int length)
        {
            var random = new Random();
            //string chars = "(ABCDEFGHIJKLMNOPQRSTUVWXYZ)+01234*56789=!_abcdefghijklmnopqrstuvwxyz?";
            // + ( ) in the begining of the name doesnt show pic
            string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}