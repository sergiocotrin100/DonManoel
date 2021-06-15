using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public static class Settings
    {
        public const string TokenKey = "DonToken";
        public const string CriptoKey = "CW$a365@";
        public const string SecretKey = "fedaf7d8863b48e197b9287d492b708e";
        public const int TimeMinutsTokenResetPassword = 30;

        public static class Role
        {
            public const string Gerente = "GER";
            public const string Cozinha = "COZ";
            public const string Caixa = "CAI";
            public const string Garcon = "GAR";
        }

        public static class Status
        {
            public static class Pedido
            {
                public const int AguardandoPreparacao = 1;
                public const int EmPreparacao = 2;
                public const int Pronto = 3;
                public const int ContaFechada = 4;
                public const int Pago = 5;
                public const int Cancelado = 6;
            }

            public static class PedidoItem
            {
                public const int Solicitado = 1;
                public const int Substituído = 2;
                public const int Cancelado = 3;
            }
        }
    }
}
