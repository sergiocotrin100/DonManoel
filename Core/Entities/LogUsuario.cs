using System;

namespace Core.Entities
{
    public class LogUsuario
    {
        public long Id { get; set; }
        public long IdUsuario { get; set; }
        public int IdPerfilUsuario { get; set; }
        public DateTime Data { get; set; }
    }
}
