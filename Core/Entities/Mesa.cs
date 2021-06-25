
using System;

namespace Core.Entities
{
    public class Mesa
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public int Numero { get; set; }
        public string Uso { get; set; }
        public bool EmUso
        {
            get
            {
                if (this.Id > 0)
                {
                    if(!string.IsNullOrWhiteSpace(this.Uso))
                    {
                        return this.Uso.Equals("S", System.StringComparison.OrdinalIgnoreCase);
                    }
                }

                return false;
            }
        }
        public string Atendente { get; set; }
        public long? IdPedido { get; set; }
        public DateTime? DataAberturaPedido { get; set; }
        public double? ValorPedido { get; set; }
        public int TaxaServico { get; set; }
        public double ValorTotalPedido
        {
            get
            {
                if(this.Id>0 && this.ValorPedido.HasValue && this.TaxaServico>0)
                {
                    var valortaxa = (this.TaxaServico * this.ValorPedido.Value) / 100;
                    return this.ValorPedido.Value + valortaxa;
                }
                return 0;
            }
        }
        public string Tempo
        {
            get
            {
                if(this.Id > 0)
                {
                    if(this.DataAberturaPedido.HasValue && (this.DataAberturaPedido.Value>DateTime.MinValue))
                    {
                        TimeSpan diff = DateTime.Now - this.DataAberturaPedido.Value;
                        return $"{diff.Hours.ToString("00")}h{diff.Minutes.ToString("00")}m";
                    }
                }
                return "";
            }
        }
       
    }
}
