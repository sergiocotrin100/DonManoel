using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class PedidoItem
    {
        public PedidoItem()
        {
            this.Excecao = new List<PedidoItemExcecao>();
            this.Menu = new Menu();
        }
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public long IdPedido { get; set; }
        public long IdMenu { get; set; }
        public long IdStatusPedidoItem { get; set; }
        public int TempoPreparo { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public List<PedidoItemExcecao> Excecao { get; set; }
        public Menu Menu { get; set; }
    }
}
