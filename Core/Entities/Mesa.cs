
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
        public DateTime? DataAberturaPedido { get; set; }
        public double? ValorPedido { get; set; }
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
