using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetUsuarios(bool? ativo);
        Task<Usuario> Login(string username, string password);
        Task<Usuario> HasAccount(string email);
        Task<Usuario> HasAccount(long idUsuario);
        Task Save(Usuario model);
        Task Update(Usuario model);
    }
}
