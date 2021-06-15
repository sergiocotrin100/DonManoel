using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CrossCutting
{
    public static class Utilities
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Shuffle(string value)
        {
            Random num = new Random();

            string rand = new string(value.ToCharArray().
                OrderBy(s => (num.Next(2) % 2) == 0).ToArray());

            return rand;
        }

        public static string EncryptPassword(string password, string key)
        {
            string encryptedPassword = string.Empty;
            var keyByte = encoding.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(encoding.GetBytes(password));
                encryptedPassword = ByteToString(hmacsha256.Hash);
            }
            return encryptedPassword;
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        public static string HandleExceptionOracle(this string message)
        {
            try
            {
                if (message.Contains("ORA-"))
                {
                    string[] str = message.Substring(11).Split("ORA-");
                    return str[0];
                }
                return message;
            }
            catch
            {
                return message;
            }
        }

        public static string RemoveAcentos(this string texto)
        {
            return new string(texto
        .Normalize(NormalizationForm.FormD)
        .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
        .ToArray());
        }

        public static string GetDescriptionMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Janeiro";
                case 2:
                    return "Fevereiro";
                case 3:
                    return "Março";
                case 4:
                    return "Abril";
                case 5:
                    return "Maio";
                case 6:
                    return "Junho";
                case 7:
                    return "Julho";
                case 8:
                    return "Agosto";
                case 9:
                    return "Setembro";
                case 10:
                    return "Outubro";
                case 11:
                    return "Novembro";
                case 12:
                    return "Dezembro";                
                default:
                    return "";
            }
        }

    }
}
