using System;

namespace Core.Entities
{
    public class LogPedidoStatus
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdPedido { get; set; }
        public long IdStatusPedido { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
    }
}
