using System;

namespace Core.Entities
{
    public class MenuComposicao
    {
        public long Id { get; set; }
        public long IdMenu { get; set; }
        public long IdUsuario { get; set; }
        public string Descricao { get; set; }
        public string Carne { get; set; }
        public bool ContemCarne
        {
            get
            {
                if(this.Id>0)
                {
                    if (!string.IsNullOrWhiteSpace(this.Carne))
                        return this.Carne.Equals("S", StringComparison.OrdinalIgnoreCase) || this.Carne.Equals("1", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
        }
        public bool Selecionado { get; set; }
    }
}
