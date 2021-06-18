
using System.Collections.Generic;

namespace Core.Entities
{
    public class Categoria
    {
        public Categoria()
        {
            this.Menu = new List<Menu>();
        }
        public int Id { get; set; }
        public long IdUsuario { get; set; }
        public string Ativo { get; set; }
        public string Nome { get; set; }
        public int Ordenacao { get; set; }
        public virtual ICollection<Menu> Menu { get; set; }
    }
}
