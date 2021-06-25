using System;
using System.Collections.Generic;
using CrossCutting;

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
        public string PontoCarne { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int Quantidade { get; set; }
        public List<PedidoItemExcecao> Excecao { get; set; }
        public Menu Menu { get; set; }
        public string ValorFormatado
        {
            get
            {
                if(this.Id>0)
                {
                    return this.Valor.FormatMoney(false);
                }
                return "";
            }
        }
        public string ValorTotalFormatado
        {
            get
            {
                if (this.Id > 0 && this.Quantidade>0)
                {
                    return (this.Quantidade * this.Valor).FormatMoney(false);
                }
                return "";
            }
        }
    }
}
