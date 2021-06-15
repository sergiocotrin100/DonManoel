using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Pedido
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdMesa { get; set; }
        public long IdStatusPedido { get; set; }
        public string Cliente { get; set; }
        public int TaxaServico { get; set; }
        public decimal ValorItens { get; set; }
        public decimal ValorTaxaServico { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public List<PedidoItem> Itens { get; set; }
        public List<LogPedidoStatus> LogStatus { get; set; }
    }
}
