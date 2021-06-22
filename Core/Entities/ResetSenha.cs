using System;

namespace Core.Entities
{
    public class ResetSenha
    {
        public long Id { get; set; }
        public long  IdUsuario { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string Token { get; set; }
    }
}
