using CrossCutting;

namespace Core.Entities
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int IdPerfilUsuario
        {
            get
            {
                if (this.Id > 0)
                {
                    switch (this.Role)
                    {
                        case Settings.Role.Cozinha:
                            {
                                return 1;
                            }
                        case Settings.Role.Garcon:
                            {
                                return 2;
                            }
                        case Settings.Role.Caixa:
                            {
                                return 3;
                            }
                        case Settings.Role.Gerente:
                            {
                                return 4;
                            }
                        default:
                            break;
                    }
                }
                    return 0;
            }
        }
        public string Departamento { get; set; }
        public string Role
        {
            get
            {
                if(this.Id>0)
                    if (!string.IsNullOrWhiteSpace(this.Departamento))
                    {
                        switch (this.Departamento.ToUpper())
                        {
                            case "GERENTE DE RESTAURANTE - MEI":
                                {
                                    return Settings.Role.Gerente;
                                }
                            case "GERENTE":
                                {
                                    return Settings.Role.Gerente;
                                }
                            case "GARÇONS":
                                {
                                    return Settings.Role.Garcon;
                                }
                            case "GARÇON":
                                {
                                    return Settings.Role.Garcon;
                                }
                            case "COZINHEIROS":
                                {
                                    return Settings.Role.Cozinha;
                                }
                            case "COZINHA":
                                {
                                    return Settings.Role.Cozinha;
                                }
                            case "CAIXA":
                                {
                                    return Settings.Role.Caixa;
                                }
                            default:
                                return "";

                        }
                    }
                return "";
            }
        }

    }
}
