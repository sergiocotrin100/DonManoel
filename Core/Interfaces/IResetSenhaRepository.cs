using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IResetSenhaRepository
    {
        Task Save(long idUser, string token);
        Task<ResetSenha> GetReset(string idUser);
    }
}
