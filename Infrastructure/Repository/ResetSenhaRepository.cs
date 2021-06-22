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
    public class ResetSenhaRepository : IResetSenhaRepository
    {
        private readonly IDonConnection _connection;
        private readonly IUserSession _userSession;
        public ResetSenhaRepository(IDonConnection connection, IUserSession userSession)
        {
            _connection = connection;
            _userSession = userSession;
        }

        public async Task Save(long idUser, string token)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_RESET_SENHA(ID_USUARIO,TOKEN)VALUES(:ID_USUARIO, :TOKEN)");
              
                var parameters = new OracleDynamicParameters();
                parameters.Add("ID_USUARIO", idUser, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("TOKEN", token, OracleDbType.NVarchar2, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }

        public async Task<ResetSenha> GetReset(string token)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@$"SELECT ID as Id, ID_USUARIO as IdUsuario, DATA_SOLICITACAO as DataSolicitacao, TOKEN as Token FROM DOTNET_RESET_SENHA WHERE TOKEN = '{token}'");
                var parametros = new OracleDynamicParameters();
                parametros.Add("TOKEN", token, OracleDbType.Varchar2);
                var resetSenhas = await conn.QuerySingleOrDefaultAsync<ResetSenha>(cmd.ToString());
                return resetSenhas;
            }
        }
    }
}
