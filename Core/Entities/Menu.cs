using System.Collections.Generic;
using CrossCutting;

namespace Core.Entities
{
   public class Menu
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public  int? IdCategoria { get; set; }
        public long IdUsuario { get; set; }
        public string Ativo { get; set; }
        public int TempoPreparo { get; set; }
        public string TipoCategoria { get; set; }
        public string PratoKids { get; set; }
        public bool IsPratoKids
        {
            get
            {
                if(this.Id>0)
                {
                    if (!string.IsNullOrWhiteSpace(this.PratoKids))
                        return this.PratoKids.Equals("S", System.StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
        }
        public decimal Valor { get; set; }
        public string ValorFormatado
        {
            get
            {
                return this.Valor.FormatMoney(false);
            }
        }
        public List<MenuComposicao> Composicao { get; set; }
    }
}
