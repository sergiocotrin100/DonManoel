using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace DonManoel.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O campo Usuário é obrigatório")]
        [DisplayName("Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        public string Senha { get; set; }

        public string ConfirmarSenha { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
