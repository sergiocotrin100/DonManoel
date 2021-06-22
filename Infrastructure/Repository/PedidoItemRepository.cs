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
        private readonly IUserSession _userSession;

        public PedidoItemRepository(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public PedidoItemRepository(IDonConnection connection, IUserSession userSession)
        {
            _connection = connection;
            _userSession = userSession;
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

        public async Task<List<PedidoItem>> GetItens(Pedido model)
        {
            var itens = new List<PedidoItem>();
            using (IDbConnection conn = _connection.GetConnection())
            {
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        itens = await GetItens(model.Id, conn, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

            return itens;
        }

        internal async Task<List<PedidoItem>> GetItens(long idpedido, IDbConnection conn, IDbTransaction transaction)
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
            var itens = await conn.QueryAsync<PedidoItem>(cmd.ToString(), parametros, transaction);

            foreach (var item in itens.ToList())
            {
                item.Menu = await GetMenu(item.IdMenu, conn, transaction);
                item.Excecao = await GetExcecao(item.Id, conn, transaction);
            }

            return itens.ToList();
        }

        private async Task<Menu> GetMenu(long idmenu, IDbConnection conn, IDbTransaction transaction)
        {
            var cmd = new StringBuilder();
            cmd.AppendFormat(@"
                            SELECT 
                                M.ID,
                                M.NOME,
                                M.DESCRICAO,
                                M.ID_USUARIO IDUSUARIO,
                                M.ATIVO,
                                M.TEMPO_PREPARO TEMPOPREPARO,
                                M.VALOR,
                                M.ID_CATEGORIA IDCATEGORIA
                            FROM MENU M
                            WHERE M.ID = :ID_MENU
                         ");
            var parametros = new DynamicParameters();
            parametros.Add("ID_MENU", idmenu, DbType.Int64);
            var menu = await conn.QuerySingleAsync<Menu>(cmd.ToString(), parametros,transaction);

            menu.Composicao = await GetMenuComposicao(idmenu, conn, transaction);

            return menu;
        }

        private async Task<List<MenuComposicao>> GetMenuComposicao(long idmenu, IDbConnection conn, IDbTransaction transaction)
        {
            var cmd = new StringBuilder();
            cmd.AppendFormat(@"
                            SELECT 
                                ID,
                                ID_MENU IDMENU,
                                DESCRICAO,
                                CARNE,
                                ID_USUARIO IDUSUARIO
                            FROM MENU_COMPOSICAO
                            WHERE ID_MENU = :ID_MENU
                         ");
            var parametros = new DynamicParameters();
            parametros.Add("ID_MENU", idmenu, DbType.Int64);
            var result = await conn.QueryAsync<MenuComposicao>(cmd.ToString(), parametros, transaction);

            return result.ToList();
        }

        private async Task<List<PedidoItemExcecao>> GetExcecao(long idpedidoitem, IDbConnection conn, IDbTransaction transaction)
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
            var model = await conn.QueryAsync<PedidoItemExcecao>(cmd.ToString(), parametros,transaction);
            return model.ToList();
        }

        public async Task Save(PedidoItem model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
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
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_ITENS(ID_PEDIDO,ID_USUARIO,ID_MENU,ID_STATUS_PEDIDO_ITEM,VALOR,TEMPO_PREPARO,OBSERVACAO,PONTO_CARNE)");
            sql.Append("VALUES(:ID_PEDIDO,:ID_USUARIO,:ID_MENU,:ID_STATUS_PEDIDO_ITEM,:VALOR,:TEMPO_PREPARO, :OBSERVACAO, :PONTO_CARNE)");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_MENU", model.IdMenu, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_STATUS_PEDIDO_ITEM", model.IdStatusPedidoItem, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("VALOR", model.Valor, OracleDbType.Decimal, ParameterDirection.Input);
            parameters.Add("TEMPO_PREPARO", model.TempoPreparo, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            parameters.Add("PONTO_CARNE", model.PontoCarne, OracleDbType.Varchar2, ParameterDirection.Input);
            var result =  await conn.ExecuteAsync(sql.ToString(), parameters, transaction);
            if (result <= 0)
                throw new Exception("Ocorreu um erro ao cadastrar o item do pedido");

            sql = new StringBuilder();
            sql.AppendFormat(@"
                            SELECT MAX(ID) ID FROM DOTNET_PEDIDO_ITENS WHERE ID_PEDIDO :ID_PEDIDO
                        ");
            parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
            var peditotem = await conn.QueryFirstAsync<PedidoItem>(sql.ToString(), parameters);

            if (peditotem.Id <= 0)
                throw new Exception("Erro ao cadastrar o pedido");

            foreach (var item in model.Excecao)
            {
                if(!string.IsNullOrWhiteSpace(item.Observacao))
                {
                    item.IdPedidoItem = peditotem.Id;
                    await SaveExcecao(item, conn, transaction);
                }
            }      
        }

        private async Task SaveExcecao(PedidoItemExcecao model, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_ITENS_EXCECAO(ID_PEDIDO_ITEM,ID_USUARIO,OBSERVACAO)");
            sql.Append("VALUES(:ID_PEDIDO_ITEM,:ID_USUARIO,:OBSERVACAO)");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO_ITEM", model.IdPedidoItem, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction);
        }
    }
}
