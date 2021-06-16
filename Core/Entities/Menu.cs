using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public class Menu
    {
        public long Id { get; set; }
        public long Nome { get; set; }
        public long Descricao { get; set; }
        public  int? IdCategoria { get; set; }
        public long IdUsuario { get; set; }
        public string Ativo { get; set; }
        public int TempoPreparo { get; set; }
        public decimal Valor { get; set; }
    }
}
