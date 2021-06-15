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
    public class PedidoItemRepository : IPedidoItemRepository
    {
        private readonly IDonConnection _connection;

        public PedidoItemRepository()
        {
        }

        public PedidoItemRepository(IDonConnection connection)
        {
            _connection = connection;
        }

        public async Task ChangeState(long id, int status)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                StringBuilder sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO_ITENS SET ID_STATUS_PEDIDO_ITEM = :STATUS WHERE ID=:ID;");
                var parameters = new OracleDynamicParameters();
                parameters.Add("ID", id, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("STATUS", status, OracleDbType.Int32, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters);
            }
        }

        public async Task<PedidoItemExcecao> GetExcecao(long idpedidoitem)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        I.ID,
                        I.ID_PEDIDO_ITEM IDPEDIDOITEM,
                        I.ID_USUARIO IDUSUARIO,
                        I.OBSERVACAO,
                        I.DATA
                    FROM DOTNET_PEDIDO_ITENS_EXCECAO I
                    WHERE I.ID_PEDIDO_ITEM=:IDPEDIDOITEM
                 ");
                var parametros = new DynamicParameters();
                parametros.Add(":IDPEDIDOITEM", idpedidoitem, DbType.Int64);
                var model = await conn.QueryFirstAsync<PedidoItemExcecao>(cmd.ToString(), parametros);
                return model;
            }
        }

        public async Task<PedidoItem> GetItemById(long id)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        I.ID,
                        I.ID_PEDIDO IDPEDIDO,
                        I.ID_USUARIO IDUSUARIO,
                        I.ID_MENU IDMENU,
                        I.ID_STATUS_PEDIDO_ITEM IDSTATUSPEDIDOITEM,
                        I.VALOR,
                        I.TEMPO_PREPARO TEMPOPREPARO,
                        I.DATA,
                        S.NOME AS STATUS
                    FROM DOTNET_PEDIDO_ITENS I
                    INNER JOIN DOTNET_STATUS_PEDIDO_ITENS S ON S.ID = I.ID_STATUS_PEDIDO_ITEM
                    WHERE I.ID=:ID
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("ID", id, DbType.String);
                var model = await conn.QueryFirstAsync<PedidoItem>(cmd.ToString(), parametros);
                return model;
            }
        }

        public async Task<List<PedidoItem>> GetItens(long idpedido)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        I.ID,
                        I.ID_PEDIDO IDPEDIDO,
                        I.ID_USUARIO IDUSUARIO,
                        I.ID_MENU IDMENU,
                        I.ID_STATUS_PEDIDO_ITEM IDSTATUSPEDIDOITEM,
                        I.VALOR,
                        I.TEMPO_PREPARO TEMPOPREPARO,
                        I.DATA,
                        S.NOME AS STATUS
                    FROM DOTNET_PEDIDO_ITENS I
                    INNER JOIN DOTNET_STATUS_PEDIDO_ITENS S ON S.ID = I.ID_STATUS_PEDIDO_ITEM
                    WHERE I.ID_PEDIDO=:IDPEDIDO
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("IDPEDIDO", idpedido, DbType.String);
                var model = await conn.QueryAsync<PedidoItem>(cmd.ToString(), parametros);
                return model.ToList();
            }
        }

        public async Task Save(PedidoItem model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                // conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await Save(model, conn,transaction);

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

        internal async Task Save(PedidoItem model, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_ITENS(ID_PEDIDO,ID_USUARIO,ID_MENU,ID_STATUS_PEDIDO_ITEM,VALOR,TEMPO_PREPARO,OBSERVACAO)");
            sql.Append("VALUES(:ID_PEDIDO,:ID_USUARIO,:ID_MENU,:ID_STATUS_PEDIDO_ITEM,:VALOR,:TEMPO_PREPARO, :OBSERVACAO);");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", model.IdUsuario, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_MENU", model.IdMenu, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_STATUS_PEDIDO_ITEM", model.IdStatusPedidoItem, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("VALOR", model.Valor, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("TEMPO_PREPARO", model.TempoPreparo, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            long idpedidoitem =  await conn.ExecuteAsync(sql.ToString(), parameters, transaction);
            if (idpedidoitem <= 0)
                throw new Exception("Ocorreu um erro ao cadastrar o item do pedido");

            if(model.PedidoItemExcecao != null && !string.IsNullOrWhiteSpace(model.PedidoItemExcecao.Observacao))
            {
                model.PedidoItemExcecao.IdPedidoItem = idpedidoitem;
                await SaveExcecao(model.PedidoItemExcecao, conn, transaction);
            }         
        }

        private async Task SaveExcecao(PedidoItemExcecao model, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_ITENS_EXCECAO(ID_PEDIDO_ITEM,ID_USUARIO,OBSERVACAO)");
            sql.Append("VALUES(:ID_PEDIDO_ITEM,:ID_USUARIO,:OBSERVACAO);");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO_ITEM", model.IdPedidoItem, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", model.IdUsuario, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction);
        }
    }
}
