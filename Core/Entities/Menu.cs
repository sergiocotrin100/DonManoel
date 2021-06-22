using System.Collections.Generic;

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
        public decimal Valor { get; set; }
        public List<MenuComposicao> Composicao { get; set; }//receita
    }
}
