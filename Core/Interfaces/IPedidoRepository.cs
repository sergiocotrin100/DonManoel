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
        Task Save(Pedido model);
        Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido);
        Task<List<Pedido>> GetPedidos(Pedido model);
        Task<List<Pedido>> GetPedidosCozinha();
        Task<Pedido> GetPedidoById(long idpedido);
        Task ChangeState(long idpedido, int status);
        Task<List<Categoria>> GetCategorias();
        Task<int> GetQuantidadePedidosPendentes(long? idusuario);
        Task<List<Pedido>> GetPedidosPendentes(long? idusuario);
    }
}
