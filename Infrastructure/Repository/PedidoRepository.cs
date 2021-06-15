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
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IDonConnection _connection;
        public PedidoRepository(IDonConnection connection)
        {
            _connection = connection;
        }

        public async Task ChangeState(long idpedido, int status)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                StringBuilder sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET ID_STATUS_PEDIDO = :STATUS WHERE ID=:ID;");
                var parameters = new OracleDynamicParameters();
                parameters.Add("ID", idpedido, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("STATUS", status, OracleDbType.Int32, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }

        public async Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        L.ID,
                        L.ID_USUARIO IDUSUARIO,
                        L.ID_STATUS_PEDIDO IDSTATUSPEDIDO,
                        L.OBSERVACAO,
                        L.DATA,
                        L.ID_PEDIDO IDPEDIDO,
                        S.NOME AS STATUS
                    FROM DOTNET_PEDIDO_LOG_STATUS L
                    INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = L.ID_STATUS_PEDIDO
                    WHERE L.ID_PEDIDO=:ID_PEDIDO
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("ID_PEDIDO", idpedido, DbType.String);
                var model = await conn.QueryAsync<LogPedidoStatus>(cmd.ToString(), parametros);
                return model.ToList();
            }
        }

        public async Task<Pedido> GetPedidoById(long idpedido)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        P.ID,
                        P.ID_USUARIO IDUSUARIO,
                        P.ID_MESA IDMESA,
                        P.CLIENTE,
                        P.ID_STATUS_PEDIDO IDSTATUSPEDIDO,
                        P.TAXA_SERVICO TAXASERVICO,
                        P.VALOR_ITENS,
                        P.VALOR_TOTAL VALORTOTAL,
                        P.DATA,
                        S.NOME AS STATUS
                    FROM DOTNET_PEDIDO P
                    INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                    WHERE P.ID=:ID
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("ID", idpedido, DbType.String);
                var model = await conn.QueryFirstAsync<Pedido>(cmd.ToString(), parametros);
                return model;
            }
        }

        public async Task<List<Pedido>> GetPedidos(Pedido item)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        P.ID,
                        P.ID_USUARIO IDUSUARIO,
                        P.ID_MESA IDMESA,
                        P.CLIENTE,
                        P.ID_STATUS_PEDIDO IDSTATUSPEDIDO,
                        P.TAXA_SERVICO TAXASERVICO,
                        P.VALOR_ITENS,
                        P.VALOR_TOTAL VALORTOTAL,
                        P.DATA,
                        S.NOME AS STATUS
                    FROM DOTNET_PEDIDO P
                    INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                    WHERE P.ID_USUARIO=:ID_USUARIO
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("ID_USUARIO", item.IdUsuario, DbType.Int64);
                var model = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);
                return model.ToList();
            }
        }

        public async Task Save(Pedido model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
               // conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO(ID_MESA,ID_USUARIO,CLIENTE,ID_STATUS_PEDIDO,TAXA_SERVICO,OBSERVACAO)");
                        sql.Append("VALUES(:ID_MESA,:ID_USUARIO,:CLIENTE,:ID_STATUS_PEDIDO,:TAXA_SERVICO, :OBSERVACAO);");
                        var parameters = new OracleDynamicParameters();
                        parameters.Add("ID_MESA", model.IdMesa, OracleDbType.Long, ParameterDirection.Input);
                        parameters.Add("ID_USUARIO", model.IdUsuario, OracleDbType.Long, ParameterDirection.Input);
                        parameters.Add("CLIENTE", model.Cliente, OracleDbType.Varchar2, ParameterDirection.Input);
                        parameters.Add("ID_STATUS_PEDIDO", model.IdStatusPedido, OracleDbType.Int32, ParameterDirection.Input);
                        parameters.Add("TAXA_SERVICO", model.TaxaServico, OracleDbType.Int32, ParameterDirection.Input);
                        parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
                        model.Id = await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);
                        if (model.Id <= 0)
                            throw new Exception("Erro ao cadastrar o pedido");

                        await SaveLogStatus(new LogPedidoStatus()
                        {
                            IdPedido=model.Id,
                            IdStatusPedido = model.IdStatusPedido,
                            IdUsuario= model.IdUsuario
                        }, conn, transaction);

                        PedidoItemRepository pedidoItemRepository = new PedidoItemRepository();
                        foreach (var item in model.Itens)
                        {
                            item.IdPedido = model.Id;
                            await pedidoItemRepository.Save(item,conn,transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public async Task SaveLogStatus(LogPedidoStatus model, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_LOG_STATUS(ID_PEDIDO,ID_USUARIO,ID_STATUS_PEDIDO,OBSERVACAO)");
            sql.Append("VALUES(:ID_PEDIDO,:ID_USUARIO,:ID_STATUS_PEDIDO,:OBSERVACAO);");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", model.IdUsuario, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_STATUS_PEDIDO", model.IdStatusPedido, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, commandType: CommandType.StoredProcedure,
                                                           transaction: transaction);
        }
    }
}
