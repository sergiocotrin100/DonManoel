
namespace Core.Entities
{
    public class Mesa
    {
        public long IdMesa { get; set; }
        public string Nome { get; set; }
        public int Numero { get; set; }
        public string Uso { get; set; }
        public bool EmUso
        {
            get
            {
                if (this.IdMesa > 0)
                {
                    if(!string.IsNullOrWhiteSpace(this.Uso))
                    {
                        return this.Uso.Equals("S", System.StringComparison.OrdinalIgnoreCase);
                    }
                }

                return false;
            }
        }
    }
}
