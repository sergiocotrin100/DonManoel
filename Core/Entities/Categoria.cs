
namespace Core.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public long IdUsuario { get; set; }
        public string Ativo { get; set; }
        public string Nome { get; set; }
    }
}
