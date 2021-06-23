using Core.Interfaces;
using Core.Entities;
using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using CrossCutting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DonManoel.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SalesController : MainController
    {
        private readonly IUserSession _userSession;
        private readonly IPedidoRepository service;
        public SalesController(IUserSession userSession, IPedidoRepository service):base(userSession,service)
        {
            _userSession = userSession;
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Order")]
        public IActionResult Order([FromQuery]long idmesa, [FromQuery] long? idorder)
        {
            Pedido model = new Pedido();
            model.IdMesa = idmesa;
            if (idorder.HasValue && idorder.Value > 0)
            {
                model = service.GetPedidoById(idorder.Value).Result;
            }

            ViewBag.Categorias = service.GetCategorias().Result;

            ViewBag.IdMesa = idmesa;
            ViewBag.IdPedido = model.Id;

            return View(model);
        }

        [HttpPost("AddItem")]
        public async Task<JsonResult> AddItem(int idmesa, string pontoCarne, string observacao, Menu menu)
        {
            Pedido model = await service.GetPedidoAbertoByMesa(idmesa);
            if (model == null || model.Id == 0)
                model = new Pedido();

            model.IdUsuario = _userSession.Id;
            model.IdMesa = idmesa;
            model.IdStatusPedido = Settings.Status.Pedido.Pendente;
            model.Itens = new List<PedidoItem>();
            PedidoItem item = new PedidoItem();
            item.IdMenu = menu.Id;
            item.IdStatusPedidoItem = Settings.Status.PedidoItem.Solicitado;
            item.IdUsuario = _userSession.Id;
            item.Observacao = observacao;            
            item.TempoPreparo = menu.TempoPreparo;
            item.Valor = menu.Valor;
            item.Excecao = new List<PedidoItemExcecao>();
            if(menu.Composicao != null && menu.Composicao.Count>0)
            {
                foreach (var excecao in menu.Composicao.Where(x => x.Selecionado == false).ToList())
                {
                    item.Excecao.Add(new PedidoItemExcecao()
                    {
                        IdUsuario = _userSession.Id,
                        Observacao = excecao.Descricao
                    });
                }

                if(menu.Composicao.Exists(item => item.ContemCarne))
                {
                    item.PontoCarne = GetPontoCarne(pontoCarne.ToInt());
                }
            }           

            model.Itens.Add(item);

            long idpedido = await service.Save(model);
            return Json(true);
        }

        [HttpGet("kitchen")]
        public IActionResult kitchen()
        {
            var pedidos = service.GetPedidosCozinha().Result;
            return View(pedidos);
        }

        [HttpGet("Tracking")]
        public IActionResult Tracking()
        {
            return View();
        }

        private string GetPontoCarne(int pontoCarne)
        {
            switch (pontoCarne)
            {
                case 1:
                    {
                        return "Selada";
                    }
                case 2:
                    {
                        return "Mau passada";
                    }
                case 3:
                    {
                        return "Ao ponto";
                    }
                case 4:
                    {
                        return "Ao ponto para bem";
                    }
                case 5:
                    {
                        return "Bem passada";
                    }
                default:
                    return "";
            }
        }

        [HttpPost("ChangeStatus")]
        public async Task<JsonResult> ChangeStatus(long idpedido, int status)
        {
            await service.ChangeState(idpedido, status);
            return Json(true);
        }

    }

    
}
