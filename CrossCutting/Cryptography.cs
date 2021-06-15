using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CrossCutting
{
    public static class Cryptography
    {
        private static Byte[] chave = { };
        private static Byte[] iv = { 12, 34, 56, 78, 90, 102, 114, 126 };

        public static string Encrypt(string valor)
        {

            if (!string.IsNullOrEmpty(valor))
            {

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs;
                Byte[] input;

                input = Encoding.UTF8.GetBytes(valor);
                chave = Encoding.UTF8.GetBytes(Settings.CriptoKey);

                cs = new CryptoStream(ms, des.CreateEncryptor(chave, iv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            else
            {
                return string.Empty;
            }

        }

        public static string Decrypt(string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                try
                {
                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs;
                    Byte[] input;

                    input = Convert.FromBase64String(valor.Replace(" ", "+"));
                    chave = Encoding.UTF8.GetBytes(Settings.CriptoKey);

                    cs = new CryptoStream(ms, des.CreateDecryptor(chave, iv), CryptoStreamMode.Write);
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch(Exception ex)
                {
                    throw new Exception("Segurança : Não permitido acesso a informação.\nCredenciais de segurança inválidos.");
                }
            }
            else
            {
                throw new Exception("Segurança : Valor NULO inválido para esta operação");
            }
        }
    }
}
