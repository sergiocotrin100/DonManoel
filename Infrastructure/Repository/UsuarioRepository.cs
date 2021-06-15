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
        public UsuarioRepository(IDonConnection connection)
        {
            _connection = connection;
        }

        public async Task<Usuario> Login(string username, string password)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"SELECT * FROM");
                var parametros = new DynamicParameters();
                parametros.Add(":username", username, DbType.String);
                parametros.Add(":password", password, DbType.String);
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
                parameters.Add("IDUSUARIO", model.Id, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("IDPERFILACESSO", model.IdPerfilUsuario, OracleDbType.Int32, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }
    }
}
