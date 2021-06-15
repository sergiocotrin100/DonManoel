using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(string username, string password);
        Task Save(Usuario model);
    }
}
