using System;

namespace Core.Entities
{
    public class PedidoItem
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdPedido { get; set; }
        public long IdMenu { get; set; }
        public long IdStatusPedidoItem { get; set; }
        public int TempoPreparo { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public PedidoItemExcecao PedidoItemExcecao { get; set; }
    }
}
