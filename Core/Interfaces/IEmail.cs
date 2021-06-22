using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Entities
{
    public interface IEmail
    {
        Task EnviarEmailAsync(string email, string assunto, string mensagem, List<string> lstEmailsCopia = null);

        Task EnviarEmailAsync(string email, string subject, string body, string username, string password, string frommail, string fromname, bool sMTPAuth, int port, string host, string anexo, List<string> lstEmailsCopia = null);
    }
}
