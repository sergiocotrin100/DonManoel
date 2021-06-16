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
                            CHR( SUBSTR( SENHA, 1, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 4, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 7, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 10, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 13, 3 ) ) || 
                            CHR( SUBSTR( SENHA, 16, 3 ) ) AS SENHA, 
                            DEPARTAMENTO 
                        FROM PCO_USR
                        WHERE FILIAL = 'D1' 
                        AND LOGIN = :USERNAME
                    )

                    SELECT * FROM VW_USUARIOS WHERE SENHA=:PASSWORD;
                ");
                var parametros = new DynamicParameters();
                parametros.Add("USERNAME", username, DbType.String);
                parametros.Add("PASSWORD", password, DbType.String);
                var model = await conn.QueryFirstAsync<Usuario>(cmd.ToString(), parametros);
                return model;
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
    }
}
