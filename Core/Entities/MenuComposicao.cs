using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MenuComposicao
    {
        public long Id { get; set; }
        public long IdMenu { get; set; }
        public long IdUsuario { get; set; }
        public string Descricao { get; set; }
        public string Carne { get; set; }
    }
}
