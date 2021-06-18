using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace CrossCutting
{
    public static class Formatting
    {
        public static string FormatDate(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy");
        }
        public static string FormatDateHour(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string FormatMoney(this decimal value)
        {
            return value.FormatMoney(true);
        }
        public static string FormatMoney(this decimal value, bool withSimbolo)
        {
            var format = $"C2";
            var result   = value.ToString(format);
            if (!withSimbolo)
                return result.Replace("R$","");

            return result;
        }
        public static string FormatMoney(this double value)
        {
            return value.FormatMoney(true);
        }
        public static string FormatMoney(this double value, bool withSimbolo)
        {
            var format = $"C2";
            var result = value.ToString(format);
            if (!withSimbolo)
                return result.Replace("R$", "");

            return result;
        }
        public static string FormatText(this string value)
        {
            return value.ToUpper().Replace("Ã", "A").Replace("'", "").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n").Replace("''", "").Replace("%", "").Replace("=", "").Replace("|", "").Replace("Ç", "C").Replace(";", "").Replace("(", "").Replace(")", "").Replace("À", "A").Replace("Á", "A").Replace("Â", "A").Replace("Ä", "A").Replace("É", "E").Replace("È", "E").Replace("Ê", "E").Replace("Ë", "E").Replace("Í", "I").Replace("Ì", "I").Replace("Î", "I").Replace("Ó", "O").Replace("Ò", "O").Replace("Ô", "O").Replace("Õ", "O").Replace("Ö", "O").Replace("Ú", "U").Replace("Ù", "U").Replace("Ü", "U").Replace("Û", "U").Replace("<br>", ". ");
        }
        public static string FormatPorcentage(this decimal value)
        {
            if (value % 1 == 0)
                return value.ToString("F0", CultureInfo.InvariantCulture) + "%";
            else
                return value.ToString("F2", CultureInfo.InvariantCulture) + "%";
        }

        public static string FormatPhoneNumber(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            long result = value.ToLong();
            if (result <= 0)
                return value;
            else if (value.Length == 10)
                return string.Format("{0:(##) ####-####}", result);
            else if (value.Length > 10)
                return string.Format("{0:(##) #####-####}", result);
            else
                return string.Format("{0:(###) ####-####}", value);
        }

        public static string FormatFax(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            long result = value.ToLong();
            if (result <= 0)
                return value;
            else if (value.Length == 10)
                return string.Format("{0:(##) ####-####}", result);
            else if (value.Length > 10)
                return string.Format("{0:(##) #####-####}", result);
            else
                return string.Format("{0:(###) ####-####}", value);
        }

        public static string FormatCep(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            long result = value.ToLong();
            if (result <= 0)
                return value;
            else if (value.Length == 8)
                return string.Format("{0:#####-###}", result);
            else
                return value;
        }

        public static bool IsFirstDayMonth(this DateTime dteData)
        {
            int month = dteData.Month;
            if (dteData.AddDays(-1).Month == month)
                return false;
            else
                return true;
        }

        public static bool IsLastDayMonth(this DateTime dteData)
        {
            int month = dteData.Month;
            if (dteData.AddDays(1).Month == month)
                return false;
            else
                return true;
        }

        public static int GetCountDayMonth(int mes, int ano)
        {
            System.Globalization.GregorianCalendar c = new System.Globalization.GregorianCalendar();
            int dias = c.GetDaysInMonth(ano, mes);
            return dias;
        }

        public static string FormatName(this string strNome)
        {

            if (!string.IsNullOrEmpty(strNome))
            {
                strNome = strNome.ToLower().Replace(@"'", @"' ").RemoveEspacosDuplos();
                System.Globalization.CultureInfo cultureinfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                strNome = cultureinfo.TextInfo.ToTitleCase(strNome);
                List<string> listaExpressoes = new List<string>() { "De", "Da", "Do", "Dos", "Das", "E" };
                for (int i = 0; i < listaExpressoes.Count; i++)
                {
                    strNome = strNome.Replace(" " + listaExpressoes[i] + " ", " " + listaExpressoes[i].ToLower() + " ");
                }
                strNome = strNome.Replace(@"' ", @"'");
            }


            return strNome;
        }

        public static string RemoveEspacosDuplos(this string value)
        {
            while (value.IndexOf("  ") >= 0)
                value = value.Replace("  ", " ");

            return value;
        }

        public static string FirstName(this string strNome)
        {
            strNome = strNome.ToLower();
            if (!string.IsNullOrEmpty(strNome))
            {
                string[] nomes = strNome.Split(' ');
                if (nomes != null && nomes.Length > 0)
                    strNome = nomes[0];
            }
            return strNome.First().ToString().ToUpper() + strNome.Substring(1);
        }

        public static string FormatCpfCnpj(this string strCpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(strCpfCnpj))
                return string.Empty;
            if (strCpfCnpj.Length <= 11)
            {

                MaskedTextProvider mtpCpf = new MaskedTextProvider(@"000\.000\.000-00");

                mtpCpf.Set(ZerosEsquerda(strCpfCnpj, 11));

                return mtpCpf.ToString();

            }

            else
            {

                MaskedTextProvider mtpCnpj = new MaskedTextProvider(@"00\.000\.000/0000-00");

                mtpCnpj.Set(ZerosEsquerda(strCpfCnpj, 11));

                return mtpCnpj.ToString();

            }

        }

        public static string MaxLength(this string objTexto, int maxLength)
        {
            if (objTexto.Length > maxLength)
                return objTexto.Substring(0, maxLength - 1);
            else
                return objTexto;
        }

        public static string ZerosEsquerda(string strString, int intTamanho)
        {

            string strResult = "";

            for (int intCont = 1; intCont <= (intTamanho - strString.Length); intCont++)
            {

                strResult += "0";

            }

            return strResult + strString;

        }

        public static bool IsBetween(this DateTime startDate, DateTime endDate, DateTime dataComparada)
        {
            if (dataComparada >= startDate && dataComparada <= endDate)
                return true;
            else
                return false;
        }

        public static string ReplaceEnter(this string strTexto, string replace)
        {
            if (!string.IsNullOrWhiteSpace(strTexto))
            {
                strTexto = strTexto.Replace("\r\n", replace);
            }
            return strTexto;
        }


    }
}
