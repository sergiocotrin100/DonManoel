using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDashBoardRepository
    {
        Task<List<GraphOrders>> GetGraphOrders(long? idusuario);
        Task<List<GraphOrders>> GetGraphOrdersByAtendente();
    }
}
