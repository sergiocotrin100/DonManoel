using Core.Entities;
using Core.Interfaces;
using CrossCutting;
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
    public class DashBoardRepository : IDashBoardRepository
    {
        private readonly IDonConnection _connection;
        private readonly IUserSession _userSession;
        public DashBoardRepository(IDonConnection connection, IUserSession userSession)
        {
            _connection = connection;
            _userSession = userSession;
        }

        public async Task<List<GraphOrders>> GetGraphOrders(long? idusuario)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        TO_CHAR( P.DATA) DATA, LOWER(TO_CHAR( P.DATA,'DAY')) DESCRICAO, COUNT(*) QUANTIDADE
                    FROM DOTNET_PEDIDO P
                    WHERE P.ID_STATUS_PEDIDO IN(:ID_STATUS_PEDIDO)
                    AND (P.ID_USUARIO = :IDUSUARIO OR :IDUSUARIO = 0)
                    AND TRUNC(P.DATA) BETWEEN TRUNC(SYSDATE - 7) AND TRUNC(SYSDATE)
                    GROUP BY TO_CHAR(P.DATA), TO_CHAR(P.DATA,'DAY')                  
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("IDUSUARIO", idusuario.HasValue ? idusuario.Value : 0, DbType.Int32, ParameterDirection.Input);
                parametros.Add("ID_STATUS_PEDIDO", Settings.Status.Pedido.Pago, DbType.Int32, ParameterDirection.Input);
                var result = await conn.QueryAsync<GraphOrders>(cmd.ToString(), parametros);
                return result.ToList();
            }
        }

        public async Task<List<GraphOrders>> GetGraphOrdersByAtendente()
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        TO_CHAR( P.DATA) DATA, LOWER(TO_CHAR( P.DATA,'DAY')) DESCRICAO, COUNT(*) QUANTIDADE
                    FROM DOTNET_PEDIDO P
                    WHERE P.ID_STATUS_PEDIDO IN(:ID_STATUS_PEDIDO)
                    AND TRUNC(P.DATA) BETWEEN TRUNC(SYSDATE - 7) AND TRUNC(SYSDATE)
                    GROUP BY TO_CHAR(P.DATA), TO_CHAR(P.DATA,'DAY')                  
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("ID_STATUS_PEDIDO", Settings.Status.Pedido.Pago, DbType.Int32, ParameterDirection.Input);
                var result = await conn.QueryAsync<GraphOrders>(cmd.ToString(), parametros);
                return result.ToList();
            }
        }
    }
}
