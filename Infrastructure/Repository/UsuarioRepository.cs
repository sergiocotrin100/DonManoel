using Core.Entities;
using Core.Interfaces;
using Dapper;
using Infrastructure.Context;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDonConnection _connection;
        private readonly IUserSession _userSession;
        public UsuarioRepository(IDonConnection connection, IUserSession userSession)
        {
            _connection = connection;
            _userSession = userSession;
        }

        public async Task<Usuario> Login(string username, string password)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"WITH VW_USUARIOS AS
                    (
                        SELECT 
                            ID,
                            LOGIN, 
                            NOME,
                            EMAIL,
                            --CHR( SUBSTR( SENHA, 1, 3 ) ) || 
                            --CHR( SUBSTR( SENHA, 4, 3 ) ) || 
                            --CHR( SUBSTR( SENHA, 7, 3 ) ) || 
                            --CHR( SUBSTR( SENHA, 10, 3 ) ) || 
                            --CHR( SUBSTR( SENHA, 13, 3 ) ) || 
                            --CHR( SUBSTR( SENHA, 16, 3 ) ) AS SENHA, 
                            SENHA,
                            DEPARTAMENTO 
                        FROM PCO_USR
                        WHERE FILIAL = 'D1' 
                        AND LOGIN = :USERNAME
                    )
select x.* from (
                    SELECT * FROM VW_USUARIOS WHERE SENHA=:PASSWORD
) x WHERE  rownum = 1
                ");
                var parametros = new OracleDynamicParameters();
                parametros.Add("USERNAME", username, OracleDbType.Varchar2, ParameterDirection.Input);
                parametros.Add("PASSWORD", FormatASCIIPass(password).Result, OracleDbType.Varchar2, ParameterDirection.Input);
                var usuarios = await conn.QuerySingleOrDefaultAsync<Usuario>(cmd.ToString(), parametros);
                return usuarios;
            }

        }

        public async Task<Usuario> HasAccount(string email)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"WITH VW_USUARIOS AS
                    (
                        SELECT 
                            ID,
                            LOGIN, 
                            NOME,
                            EMAIL,
                            CHR( SUBSTR( SENHA, 1, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 4, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 7, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 10, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 13, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 16, 3 ) ) AS SENHA, 
                            DEPARTAMENTO 
                        FROM PCO_USR
                        WHERE FILIAL = 'D1' 
                        AND EMAIL = :EMAIL
                    )
select x.* from (SELECT * FROM VW_USUARIOS WHERE EMAIL=:EMAIL) x WHERE  rownum = 1
                ");
                var parametros = new DynamicParameters();
                parametros.Add("EMAIL", email, DbType.String);
                var usuarios = await conn.QuerySingleOrDefaultAsync<Usuario>(cmd.ToString(), parametros);
                return usuarios;
            }

        }

        public async Task<Usuario> HasAccount(long idUsuario)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"WITH VW_USUARIOS AS
                    (
                        SELECT 
                            ID,
                            LOGIN, 
                            NOME,
                            EMAIL,
                            CHR( SUBSTR( SENHA, 1, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 4, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 7, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 10, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 13, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 16, 3 ) ) AS SENHA, 
                            DEPARTAMENTO 
                        FROM PCO_USR
                        WHERE FILIAL = 'D1' 
                        AND ID = :ID
                    )

                    SELECT * FROM VW_USUARIOS WHERE ID=:ID
                ");
                var parametros = new OracleDynamicParameters();
                parametros.Add("ID", idUsuario, OracleDbType.Long);
                var usuarios = await conn.QueryAsync<Usuario>(cmd.ToString(), parametros);
                return usuarios.FirstOrDefault();
            }

        }

        public async Task Save(Usuario model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_LOG_USUARIO(ID_USUARIO,ID_PERFIL_ACESSO");
                sql.Append("VALUES(:IDUSUARIO, :IDPERFILACESSO);");
                var parameters = new OracleDynamicParameters();
                parameters.Add("IDUSUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("IDPERFILACESSO", model.IdPerfilUsuario, OracleDbType.Int32, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }

        public async Task Update(Usuario model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                StringBuilder sql = new StringBuilder(@"UPDATE PCO_USR SET SENHA = :SENHA WHERE EMAIL = :EMAIL AND ID=:ID");

                var parameters = new OracleDynamicParameters();
                parameters.Add("ID", model.Id, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("EMAIL", model.Email, OracleDbType.Varchar2, ParameterDirection.Input);
                parameters.Add("SENHA", FormatASCIIPass(model.Senha).Result, OracleDbType.Varchar2, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }

        private Task<string> FormatASCIIPass(string pass)
        {
            string lineEncoded = String.Empty;
            for (int i = 0; i < pass.Length; i++)
            {
                string valor = pass.Substring(i, 1);
                switch (valor)
                {
                    case "0":
                        lineEncoded += "048";
                        break;
                    case "1":
                        lineEncoded += "049";
                        break;
                    case "2":
                        lineEncoded += "050";
                        break;
                    case "3":
                        lineEncoded += "051";
                        break;
                    case "4":
                        lineEncoded += "052";
                        break;
                    case "5":
                        lineEncoded += "053";
                        break;

                    case "6":
                        lineEncoded += "054";
                        break;
                    case "7":
                        lineEncoded += "055";
                        break;
                    case "8":
                        lineEncoded += "056";
                        break;
                    case "9":
                        lineEncoded += "057";
                        break;
                    case "!":
                        lineEncoded += "033"; break;
                    case "\"": lineEncoded += "034"; break;
                    case "#": lineEncoded += "035"; break;
                    case "$": lineEncoded += "036"; break;
                    case "%": lineEncoded += "037"; break;
                    case "&": lineEncoded += "038"; break;
                    case "'": lineEncoded += "039"; break;
                    case "(": lineEncoded += "040"; break;
                    case ")": lineEncoded += "041"; break;
                    case "*": lineEncoded += "042"; break;
                    case "+": lineEncoded += "043"; break;
                    case ",": lineEncoded += "044"; break;
                    case "-": lineEncoded += "045"; break;
                    case ".": lineEncoded += "046"; break;
                    case "/": lineEncoded += "047"; break;
                    case ":": lineEncoded += "058"; break;
                    case ";": lineEncoded += "059"; break;
                    case "<": lineEncoded += "060"; break;
                    case "=": lineEncoded += "061"; break;
                    case ">": lineEncoded += "062"; break;
                    case "?": lineEncoded += "063"; break;
                    case "@": lineEncoded += "064"; break;
                    case "A": lineEncoded += "065"; break;
                    case "B": lineEncoded += "066"; break;
                    case "C": lineEncoded += "067"; break;
                    case "D": lineEncoded += "068"; break;
                    case "E": lineEncoded += "069"; break;
                    case "F": lineEncoded += "070"; break;
                    case "G": lineEncoded += "071"; break;
                    case "H": lineEncoded += "072"; break;
                    case "I": lineEncoded += "073"; break;
                    case "J": lineEncoded += "074"; break;
                    case "K": lineEncoded += "075"; break;
                    case "L": lineEncoded += "076"; break;
                    case "M": lineEncoded += "077"; break;
                    case "N": lineEncoded += "078"; break;
                    case "O": lineEncoded += "079"; break;
                    case "P": lineEncoded += "080"; break;
                    case "Q": lineEncoded += "081"; break;
                    case "R": lineEncoded += "082"; break;
                    case "S": lineEncoded += "083"; break;
                    case "T": lineEncoded += "084"; break;
                    case "U": lineEncoded += "085"; break;
                    case "V": lineEncoded += "086"; break;
                    case "W": lineEncoded += "087"; break;
                    case "X": lineEncoded += "088"; break;
                    case "Y": lineEncoded += "089"; break;
                    case "Z": lineEncoded += "090"; break;
                    case "[": lineEncoded += "091"; break;
                    case @"\": lineEncoded += "092"; break;
                    case "]": lineEncoded += "093"; break;
                    case "^": lineEncoded += "094"; break;
                    case "_": lineEncoded += "095"; break;
                    case "`": lineEncoded += "096"; break;
                    case "a": lineEncoded += "097"; break;
                    case "b": lineEncoded += "098"; break;
                    case "c": lineEncoded += "099"; break;
                    case "d": lineEncoded += "0100"; break;
                    case "e": lineEncoded += "0101"; break;
                    case "f": lineEncoded += "0102"; break;
                    case "g": lineEncoded += "0103"; break;
                    case "h": lineEncoded += "0104"; break;
                    case "i": lineEncoded += "0105"; break;
                    case "j": lineEncoded += "0106"; break;
                    case "k": lineEncoded += "0107"; break;
                    case "l": lineEncoded += "0108"; break;
                    case "m": lineEncoded += "0109"; break;
                    case "n": lineEncoded += "0110"; break;
                    case "o": lineEncoded += "0111"; break;
                    case "p": lineEncoded += "0112"; break;
                    case "q": lineEncoded += "0113"; break;
                    case "r": lineEncoded += "0114"; break;
                    case "s": lineEncoded += "0115"; break;
                    case "t": lineEncoded += "0116"; break;
                    case "u": lineEncoded += "0117"; break;
                    case "v": lineEncoded += "0118"; break;
                    case "w": lineEncoded += "0119"; break;
                    case "x": lineEncoded += "0120"; break;
                    case "y": lineEncoded += "0121"; break;
                    case "z": lineEncoded += "0122"; break;
                    case "{": lineEncoded += "0123"; break;
                    case "|": lineEncoded += "0124"; break;
                    case "}": lineEncoded += "0125"; break;
                    case "~": lineEncoded += "0126"; break;
                }
            }
            return Task.FromResult(lineEncoded);
        }
    }
}
