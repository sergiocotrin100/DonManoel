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
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IDonConnection _connection;
        private readonly IUserSession _userSession;
        public PedidoRepository(IDonConnection connection, IUserSession userSession)
        {
            _connection = connection;
            _userSession = userSession;
        }

        public PedidoRepository()
        {

        }

        public async Task ChangeState(long idpedido, int status, string taxaservico = null)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await ChangeState(idpedido, status, !string.IsNullOrWhiteSpace(taxaservico) && taxaservico.Equals("S", StringComparison.OrdinalIgnoreCase), conn, transaction);

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

        internal async Task ChangeState(long idpedido, int status, bool taxaservico, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET ID_STATUS_PEDIDO = :STATUS WHERE ID=:ID");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID", idpedido, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("STATUS", status, OracleDbType.Int32, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);

            await SaveLogStatus(new LogPedidoStatus()
            {
                IdPedido = idpedido,
                IdStatusPedido = status
            }, conn, transaction);

            if (status == (int)Settings.Status.Pedido.EmPreparacao)
            {
                sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO_ITENS SET ID_STATUS_PEDIDO_ITEM = :STATUS WHERE ID_PEDIDO=:ID AND ID_STATUS_PEDIDO_ITEM =1");
                parameters = new OracleDynamicParameters();
                parameters.Add("ID", idpedido, OracleDbType.Long, ParameterDirection.Input);
                parameters.Add("STATUS", (int)Settings.Status.PedidoItem.Enviado, OracleDbType.Int32, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);
            }

            if (status == (int)Settings.Status.Pedido.Pago)
            {
                sql = new StringBuilder();
                if (taxaservico)
                    sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET VALOR_TAXA_SERVICO = (VALOR_ITENS * TAXA_SERVICO) /100, VALOR_TOTAL = ((VALOR_ITENS * TAXA_SERVICO) /100) + VALOR_ITENS WHERE ID=:ID");
                else
                    sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET VALOR_TAXA_SERVICO = NULL, VALOR_TOTAL = VALOR_ITENS WHERE ID=:ID");

                parameters = new OracleDynamicParameters();
                parameters.Add("ID", idpedido, OracleDbType.Long, ParameterDirection.Input);
                await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);
            }
        }

        public async Task ChangeStateItem(long idpedidoitem, int status)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await ChangeStateItem(idpedidoitem, status, conn, transaction);

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

        public async Task ChangeStateItemBar(string nomePedidoitem,long idPedido, int status, string statusFase)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        await ChangeStateItemBar(nomePedidoitem, idPedido,status, statusFase, conn, transaction);

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

        internal async Task ChangeStateItem(long idpedidoitem, int status, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO_ITENS SET ID_STATUS_PEDIDO_ITEM = :STATUS, DATA_ATUALIZACAO = SYSDATE WHERE ID=:ID");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID", idpedidoitem, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("STATUS", status, OracleDbType.Int32, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);

            if (status == (int)Settings.Status.PedidoItem.Pronto)
            {
                PedidoItemRepository pedidoitem = new PedidoItemRepository(_userSession);
                PedidoItem model = await pedidoitem.GetItemById(idpedidoitem, conn, transaction);

                sql = new StringBuilder();
                sql.AppendFormat(@"
                    SELECT 
                        I.ID_STATUS_PEDIDO_ITEM IDSTATUSPEDIDOITEM
                    FROM DOTNET_PEDIDO_ITENS I
                    WHERE I.ID_PEDIDO=:IDPEDIDO
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("IDPEDIDO", model.IdPedido, DbType.Int64);
                var itens = await conn.QueryAsync<PedidoItem>(sql.ToString(), parametros, transaction);

                int statusPedidoVenda = 0;
                if (itens.ToList().Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Enviado))
                    statusPedidoVenda = (int)Settings.Status.Pedido.EmPreparacao;
                else
                    statusPedidoVenda = (int)Settings.Status.Pedido.Pronto;

                if (statusPedidoVenda > 0)
                {
                    sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET ID_STATUS_PEDIDO = :STATUS WHERE ID=:ID");
                    parameters = new OracleDynamicParameters();
                    parameters.Add("ID", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
                    parameters.Add("STATUS", statusPedidoVenda, OracleDbType.Int32, ParameterDirection.Input);
                    await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);

                    await SaveLogStatus(new LogPedidoStatus()
                    {
                        IdPedido = model.IdPedido,
                        IdStatusPedido = status
                    }, conn, transaction);
                }
            }
        }

        
        internal async Task ChangeStateItemBar(string nomePedidoitem, long idPedido, int status, string statusFase, IDbConnection conn, IDbTransaction transaction)
        {
            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
            var todosItens = await repository.GetItens(idPedido, conn, transaction);

            //quando bebidas, logica para mudar status da bebida que esta agrupada na tela
            var listPedidoBar = new List<PedidoItem>();
            foreach (var item in todosItens.Where(item => item.Menu.TipoCategoria == Settings.TipoCategoria.Bar).ToList())
            {
                if (item.Menu.Nome == nomePedidoitem && item.IdStatusPedidoItem == statusFase.ToLong())
                {
                    listPedidoBar.Add(item);
                }
            }

            //cancela item do bar 1 por 1
            long idPedidoItem = 0;
            if (listPedidoBar.Count > 1)
            {
                foreach (var item in listPedidoBar)
                {
                    // se o valor atual da lista for maior que o valor que já tenho
                    if (item.Id > idPedidoItem)
                        // atualiza o valor que ja tenho
                        idPedidoItem = item.Id;
                }
            }
            else
            {
                idPedidoItem = listPedidoBar.FirstOrDefault().Id;
            }

            StringBuilder sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO_ITENS SET ID_STATUS_PEDIDO_ITEM = :STATUS, DATA_ATUALIZACAO = SYSDATE WHERE ID=:ID");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID", idPedidoItem, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("STATUS", status, OracleDbType.Int32, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);

            if (status == (int)Settings.Status.PedidoItem.Pronto)
            {
                PedidoItemRepository pedidoitem = new PedidoItemRepository(_userSession);
                PedidoItem model = await pedidoitem.GetItemById(idPedidoItem, conn, transaction);

                sql = new StringBuilder();
                sql.AppendFormat(@"
                    SELECT 
                        I.ID_STATUS_PEDIDO_ITEM IDSTATUSPEDIDOITEM
                    FROM DOTNET_PEDIDO_ITENS I
                    WHERE I.ID_PEDIDO=:IDPEDIDO
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("IDPEDIDO", model.IdPedido, DbType.Int64);
                var itens = await conn.QueryAsync<PedidoItem>(sql.ToString(), parametros, transaction);

                int statusPedidoVenda = 0;
                if (itens.ToList().Exists(x => x.IdStatusPedidoItem == (int)Settings.Status.PedidoItem.Enviado))
                    statusPedidoVenda = (int)Settings.Status.Pedido.EmPreparacao;
                else
                    statusPedidoVenda = (int)Settings.Status.Pedido.Pronto;

                if (statusPedidoVenda > 0)
                {
                    sql = new StringBuilder(@"UPDATE DOTNET_PEDIDO SET ID_STATUS_PEDIDO = :STATUS WHERE ID=:ID");
                    parameters = new OracleDynamicParameters();
                    parameters.Add("ID", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
                    parameters.Add("STATUS", statusPedidoVenda, OracleDbType.Int32, ParameterDirection.Input);
                    await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);

                    await SaveLogStatus(new LogPedidoStatus()
                    {
                        IdPedido = model.IdPedido,
                        IdStatusPedido = status
                    }, conn, transaction);
                }
            }
        }


        public async Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido)
        {
            List<LogPedidoStatus> lstLog = new List<LogPedidoStatus>();
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        lstLog = await GetLogPedidoStatus(idpedido, conn, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            return lstLog;
        }

        internal async Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido, IDbConnection conn, IDbTransaction transaction)
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

        public async Task<Pedido> GetPedidoById(long idpedido)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    Pedido model = new Pedido();
                    try
                    {
                        model = await GetPedidoById(idpedido, conn, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    return model;
                }
            }
        }

        internal async Task<Pedido> GetPedidoById(long idpedido, IDbConnection conn, IDbTransaction transaction)
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
                                P.VALOR_ITENS VALORITENS,
                                P.VALOR_TOTAL VALORTOTAL,
                                P.DATA,
                                S.NOME AS STATUS,
                                U.NOME ATENDENTE    
                            FROM DOTNET_PEDIDO P
                            INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                            LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                            WHERE P.ID=:ID
                         ");
            var parametros = new DynamicParameters();
            parametros.Add("ID", idpedido, DbType.Int64);
            var model = await conn.QueryFirstAsync<Pedido>(cmd.ToString(), parametros);

            model.LogStatus = await GetLogPedidoStatus(idpedido, conn, transaction);

            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
            model.Itens = await repository.GetItens(model.Id, conn, transaction);
            return model;
        }

        public async Task<List<Pedido>> GetPedidos(Pedido item)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    IEnumerable<Pedido> listPedidos = new List<Pedido>();
                    try
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
                                P.VALOR_ITENS VALORITENS,
                                P.VALOR_TOTAL VALORTOTAL,
                                P.DATA,
                                S.NOME AS STATUS,
                                U.NOME ATENDENTE    
                            FROM DOTNET_PEDIDO P
                            INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                            LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                            WHERE P.ID = NVL(:ID, P.ID)
                            AND P.ID_USUARIO = NVL(:ID_USUARIO, P.ID_USUARIO)
                            AND P.ID_MESA = NVL(:ID_MESA, P.ID_MESA)
                            AND P.ID_STATUS_PEDIDO = NVL(:ID_STATUS_PEDIDO, P.ID_STATUS_PEDIDO)
                            AND ((:DTINI IS NULL AND :DTFIM IS NULL) OR
                             (TRUNC(P.DATA) BETWEEN trunc(:DTINI) AND TRUNC(:DTFIM)))
                         ");
                        var parametros = new DynamicParameters();
                        parametros.Add("ID", item.Id > 0 ? item.Id : (object)null, DbType.Int64);
                        parametros.Add("ID_USUARIO", item.IdUsuario > 0 ? item.IdUsuario : (object)null, DbType.Int64);
                        parametros.Add("ID_MESA", item.IdMesa > 0 ? item.IdMesa : (object)null, DbType.Int64);
                        parametros.Add("ID_STATUS_PEDIDO", item.IdStatusPedido > 0 ? item.IdStatusPedido : (object)null, DbType.Int64);
                        parametros.Add("DTINI", item.DataInicio.HasValue ? item.DataInicio.Value : (object)null, DbType.Date);
                        parametros.Add("DTFIM", item.DataFim.HasValue ? item.DataFim.Value : (object)null, DbType.Date);
                        listPedidos = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);

                        foreach (var pedido in listPedidos.ToList())
                        {
                            pedido.LogStatus = await GetLogPedidoStatus(pedido.Id, conn, transaction);

                            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
                            pedido.Itens = await repository.GetItens(pedido.Id, conn, transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    return listPedidos.ToList();
                }
            }
        }

        public async Task<long> Save(Pedido model)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        StringBuilder sql = new StringBuilder();

                        if (model.Id == 0)
                        {
                            sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO(ID_MESA,ID_USUARIO,CLIENTE,ID_STATUS_PEDIDO,TAXA_SERVICO,OBSERVACAO)");
                            sql.Append("VALUES(:ID_MESA,:ID_USUARIO,:CLIENTE,:ID_STATUS_PEDIDO,:TAXA_SERVICO, :OBSERVACAO)");
                            var parameters = new OracleDynamicParameters();
                            parameters.Add("ID_MESA", model.IdMesa, OracleDbType.Long, ParameterDirection.Input);
                            parameters.Add("ID_USUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
                            parameters.Add("CLIENTE", model.Cliente, OracleDbType.Varchar2, ParameterDirection.Input);
                            parameters.Add("ID_STATUS_PEDIDO", model.IdStatusPedido, OracleDbType.Int32, ParameterDirection.Input);
                            parameters.Add("TAXA_SERVICO", model.TaxaServico, OracleDbType.Int32, ParameterDirection.Input);
                            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
                            var result = await conn.ExecuteAsync(sql.ToString(), parameters, transaction: transaction);
                            if (result <= 0)
                                throw new Exception("Erro ao cadastrar o pedido");

                            sql = new StringBuilder();
                            sql.AppendFormat(@"
                            SELECT MAX(ID) ID FROM DOTNET_PEDIDO WHERE ID_MESA=:ID_MESA AND ID_USUARIO = :ID_USUARIO
                        ");
                            parameters = new OracleDynamicParameters();
                            parameters.Add("ID_MESA", model.IdMesa, OracleDbType.Long, ParameterDirection.Input);
                            parameters.Add("ID_USUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
                            var pedido = await conn.QueryFirstAsync<Pedido>(sql.ToString(), parameters, transaction: transaction);

                            if (pedido.Id <= 0)
                                throw new Exception("Erro ao cadastrar o pedido");

                            model.Id = pedido.Id;

                            await SaveLogStatus(new LogPedidoStatus()
                            {
                                IdPedido = model.Id,
                                IdStatusPedido = model.IdStatusPedido
                            }, conn, transaction);
                        }

                        PedidoItemRepository pedidoItemRepository = new PedidoItemRepository(_userSession);
                        foreach (var item in model.Itens)
                        {
                            item.IdPedido = model.Id;
                            await pedidoItemRepository.Save(item, conn, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
            return model.Id;
        }

        private async Task SaveLogStatus(LogPedidoStatus model, IDbConnection conn, IDbTransaction transaction)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO DOTNET_PEDIDO_LOG_STATUS(ID_PEDIDO,ID_USUARIO,ID_STATUS_PEDIDO,OBSERVACAO)");
            sql.Append("VALUES(:ID_PEDIDO,:ID_USUARIO,:ID_STATUS_PEDIDO,:OBSERVACAO)");
            var parameters = new OracleDynamicParameters();
            parameters.Add("ID_PEDIDO", model.IdPedido, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_USUARIO", _userSession.Id, OracleDbType.Long, ParameterDirection.Input);
            parameters.Add("ID_STATUS_PEDIDO", model.IdStatusPedido, OracleDbType.Int32, ParameterDirection.Input);
            parameters.Add("OBSERVACAO", model.Observacao, OracleDbType.Varchar2, ParameterDirection.Input);
            await conn.ExecuteAsync(sql.ToString(), parameters, transaction);
        }

        public async Task<List<Categoria>> GetCategorias()
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        ID,
                        ID_USUARIO IDUSUARIO,
                        NOME,
                        ATIVO,
                        ORDENACAO
                    FROM CATEGORIA 
                    WHERE ATIVO='S'
                    ORDER BY ORDENACAO                    
                 ");

                var parametros = new DynamicParameters();
                var categorias = await conn.QueryAsync<Categoria>(cmd.ToString(), parametros);


                cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        ID,
                        M.ID,
                        M.NOME,
                        M.DESCRICAO,
                        M.ID_USUARIO IDUSUARIO,
                        M.ATIVO,
                        M.TEMPO_PREPARO TEMPOPREPARO,
                        M.VALOR,
                        M.ID_CATEGORIA IDCATEGORIA,
                        M.IMAGEM,
                        NVL(M.PRATO_KIDS,'N') PRATOKIDS  
                    FROM MENU M
                    WHERE M.ATIVO='S'    
                               
                 ");
                var menus = await conn.QueryAsync<Menu>(cmd.ToString(), parametros);
                menus = menus.ToList();

                foreach (var item in categorias)
                {
                    item.Menu = menus.Where(x => x.IdCategoria == item.Id).ToList();

                    foreach (var menu in item.Menu)
                    {
                        cmd = new StringBuilder();
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
                        parametros = new DynamicParameters();
                        parametros.Add("ID_MENU", menu.Id, DbType.Int64);
                        var result = await conn.QueryAsync<MenuComposicao>(cmd.ToString(), parametros);
                        menu.Composicao = result.ToList();
                    }
                }
                return categorias.ToList();
            }
        }

        public async Task<List<Pedido>> GetPedidosCozinha()
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    IEnumerable<Pedido> listPedidos = new List<Pedido>();
                    try
                    {
                        var cmd = new StringBuilder();
                        cmd.AppendFormat(@"
                            WITH ITENS AS
                            (
                                SELECT I.* FROM DOTNET_PEDIDO_ITENS I
                                INNER JOIN MENU M ON M.ID = I.ID_MENU
                                INNER JOIN CATEGORIA C ON C.ID = M.ID_CATEGORIA
                                WHERE  I.ID_STATUS_PEDIDO_ITEM = 2
                                AND C.TIPO = 'C'
                            )
                            
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
                                S.NOME AS STATUS,
                                U.NOME ATENDENTE    
                            FROM DOTNET_PEDIDO P
                            INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                            LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                            WHERE P.ID_STATUS_PEDIDO IN(2,3,4,5)
                            AND P.ID IN
                            (
                                SELECT ID_PEDIDO FROM ITENS
                            )
                         ");
                        var parametros = new DynamicParameters();
                        listPedidos = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);

                        foreach (var pedido in listPedidos.ToList())
                        {
                            pedido.LogStatus = await GetLogPedidoStatus(pedido.Id, conn, transaction);

                            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
                            pedido.Itens = await repository.GetItens(pedido.Id, conn, transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    return listPedidos.ToList();
                }
            }
        }

        public async Task<List<Pedido>> GetPedidosBar()
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    IEnumerable<Pedido> listaPedidos = new List<Pedido>();
                    try
                    {
                        var cmd = new StringBuilder();
                        cmd.AppendFormat(@"
                            WITH ITENS AS
                            (
                                SELECT I.* FROM DOTNET_PEDIDO_ITENS I
                                INNER JOIN MENU M ON M.ID = I.ID_MENU
                                INNER JOIN CATEGORIA C ON C.ID = M.ID_CATEGORIA
                                WHERE  I.ID_STATUS_PEDIDO_ITEM = 2
                                AND C.TIPO = 'B'
                            )
                            
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
                                S.NOME AS STATUS,
                                U.NOME ATENDENTE    
                            FROM DOTNET_PEDIDO P
                            INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                            LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                            WHERE P.ID_STATUS_PEDIDO IN(2,3,4,5)
                            AND P.ID IN
                            (
                                SELECT ID_PEDIDO FROM ITENS
                            )
                         ");
                        var parametros = new DynamicParameters();
                        listaPedidos = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);

                        foreach (var pedido in listaPedidos.ToList())
                        {
                            pedido.LogStatus = await GetLogPedidoStatus(pedido.Id, conn, transaction);

                            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
                            pedido.Itens = await repository.GetItens(pedido.Id, conn, transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    return listaPedidos.ToList();
                }
            }
        }

        public async Task<List<Pedido>> GetMeusPedidos(long? idusuario)
        {
            using (IDbConnection conn = _connection.GetConnection())
            {
                conn.Open();
                using (IDbTransaction transaction = conn.BeginTransaction())
                {
                    IEnumerable<Pedido> listaPedidos = new List<Pedido>();
                    try
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
                        S.NOME AS STATUS,
                        U.NOME ATENDENTE
                    FROM DOTNET_PEDIDO P
                    INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                    LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                    WHERE P.ID_STATUS_PEDIDO IN(1,2,3,4,5)
                    AND (P.ID_USUARIO = :IDUSUARIO OR :IDUSUARIO = 0)
                 ");
                        var parametros = new DynamicParameters();
                        parametros.Add("IDUSUARIO", idusuario.HasValue ? idusuario.Value : 0, DbType.Int32, ParameterDirection.Input);
                        listaPedidos = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);

                        foreach (var pedido in listaPedidos.ToList())
                        {
                            pedido.LogStatus = await GetLogPedidoStatus(pedido.Id, conn, transaction);

                            PedidoItemRepository repository = new PedidoItemRepository(_userSession);
                            pedido.Itens = await repository.GetItens(pedido.Id, conn, transaction);

                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    return listaPedidos.ToList();
                }
            }
        }

        public async Task<Pedido> GetPedidoAbertoByMesa(int idMesa)
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
                        S.NOME AS STATUS,
                        U.NOME ATENDENTE
                    FROM DOTNET_PEDIDO P
                    INNER JOIN DOTNET_STATUS_PEDIDO S ON S.ID = P.ID_STATUS_PEDIDO
                    LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                    WHERE P.ID_MESA=:IDMESA
                    AND P.ID_STATUS_PEDIDO IN(1,2,3,4,5)
                    ORDER BY P.ID DESC
                 ");
                var parametros = new DynamicParameters();
                parametros.Add("IDMESA", idMesa, DbType.Int32);
                var model = await conn.QueryAsync<Pedido>(cmd.ToString(), parametros);
                return model.FirstOrDefault();
            }
        }

    }
}
