using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
   public  interface IPedidoRepository
    {
        Task<long> Save(Pedido model);
        Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido);
        Task<List<Pedido>> GetPedidos(Pedido model);
        Task<List<Pedido>> GetPedidosCozinha();
        Task<List<Pedido>> GetPedidosBar();
        Task<Pedido> GetPedidoById(long idpedido);
        Task<Pedido> GetPedidoAbertoByMesa(int idMesa);
        Task ChangeState(long idpedido, int status, string taxaservico = null);
        Task ChangeStateItem(long idpedidoitem, int status);
        Task ChangeStateItemBar(string nomePedidoitem, long idPedido, int status,string statusFase);
        Task<List<Categoria>> GetCategorias();
        Task<List<Pedido>> GetMeusPedidos(long? idusuario);

    }
}
