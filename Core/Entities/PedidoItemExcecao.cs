using System;

namespace Core.Entities
{
    public class PedidoItemExcecao
    {
        public long Id { get; set; }
        public long IdPedidoItem { get; set; }
        public long IdUsuario { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
    }
}
