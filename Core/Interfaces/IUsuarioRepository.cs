using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(string username, string password);
        Task<Usuario> HasAccount(string email);
        Task<Usuario> HasAccount(long idUsuario);
        Task Save(Usuario model);
        Task Update(Usuario model);
    }
}
