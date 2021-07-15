using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPedidoItemRepository
    {
        Task Save(PedidoItem model);
        Task<List<PedidoItem>> GetItens(Pedido model);
        Task<PedidoItem> GetItemById(long idpedido);
        Task<PedidoItem> GetItemByNameOrder(string nomePedidoItem, long idPedido);
    }
}
