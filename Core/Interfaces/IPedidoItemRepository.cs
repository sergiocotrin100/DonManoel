﻿using Core.Entities;
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
        Task<List<PedidoItem>> GetItens(long idpedido);
        Task<PedidoItem> GetItemById(long idpedido);
        Task ChangeState(long idpedido, int status);
        Task<PedidoItemExcecao> GetExcecao(long idpedidoitem);
    }
}
