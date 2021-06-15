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
        Task SaveLogStatus(LogPedidoStatus model);
        Task<List<LogPedidoStatus>> GetLogPedidoStatus(long idpedido);
        Task<List<Pedido>> GetPedidos(Pedido model);
        Task<Pedido> GetPedidoById(long idpedido);
        Task ChangeState(long idpedido, int status);
    }
}
