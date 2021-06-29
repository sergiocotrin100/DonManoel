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
        private readonly IPedidoItemRepository serviceItem;
        private readonly IMesaRepository serviceMesa;
        private readonly IUsuarioRepository serviceUsuario;

        public SalesController(IUserSession userSession, IPedidoRepository service, IPedidoItemRepository serviceItem, IMesaRepository serviceMesa, IUsuarioRepository serviceUsuario) : base(userSession, service)
        {
            _userSession = userSession;
            this.service = service;
            this.serviceItem = serviceItem;
            this.serviceMesa = serviceMesa;
            this.serviceUsuario = serviceUsuario;
        }
        public IActionResult Index()
        {
            var model = new Pedido();
            model.DataInicio = DateTime.Now.AddDays(-10);
            model.DataFim = DateTime.Now.Date;
            if (_userSession.Role == Settings.Role.Garcon)
                model.IdUsuario = _userSession.Id;
            var result = service.GetPedidos(model).Result;
            ViewBag.Mesas = GetMesas().OrderBy(x=> x.Id).ToList();
            ViewBag.Usuarios = GeUsuarios().OrderBy(x => x.Nome).ToList();
            return View(result);
        }

        private List<Mesa> GetMesas()
        {
            return serviceMesa.GetAll().Result;
        }

        private List<Usuario> GeUsuarios()
        {
            return serviceUsuario.GetUsuarios(true).Result;
        }

        [HttpPost("GetPedidos")]
        public PartialViewResult GetPedidos(Pedido model)
        {
            if (_userSession.Role == Settings.Role.Garcon)
                model.IdUsuario = _userSession.Id;
            var result = service.GetPedidos(model).Result;
            return PartialView("_pedidos", result);
        }

        [HttpGet("Order")]
        public IActionResult Order([FromQuery] long idmesa, [FromQuery] long? idorder)
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
            try
            {
                Pedido model = await service.GetPedidoAbertoByMesa(idmesa);
                if (model == null || model.Id == 0)
                    model = new Pedido();

                if (model.IdStatusPedido == (int)Settings.Status.Pedido.Cancelado || model.IdStatusPedido == (int)Settings.Status.Pedido.Pago)
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
                if (menu.Composicao != null && menu.Composicao.Count > 0)
                {
                    foreach (var excecao in menu.Composicao.Where(x => x.Selecionado == false).ToList())
                    {
                        item.Excecao.Add(new PedidoItemExcecao()
                        {
                            IdUsuario = _userSession.Id,
                            Observacao = excecao.Descricao
                        });
                    }

                    if (menu.Composicao.Exists(item => item.ContemCarne))
                    {
                        item.PontoCarne = GetPontoCarne(pontoCarne.ToInt());
                    }
                }

                model.Itens.Add(item);

                long idpedido = await service.Save(model);
                return Json(new { success = true, message = "", result = idpedido });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("kitchen")]
        public IActionResult kitchen()
        {
            var pedidos = service.GetPedidosCozinha().Result;
            return View(pedidos);
        }

        [HttpGet("GetPedidosCozinha")]
        public PartialViewResult GetPedidosCozinha()
        {
            var pedidos = service.GetPedidosCozinha().Result;
            return PartialView("_kitchenPartial", pedidos);
        }

        [HttpGet("Bar")]
        public IActionResult Bar()
        {
            var pedidos = service.GetPedidosBar().Result;
            return View(pedidos);
        }

        [HttpGet("GetPedidosBar")]
        public PartialViewResult GetPedidosBar()
        {
            var pedidos = service.GetPedidosBar().Result;
            return PartialView("_barPartial", pedidos);
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
        public async Task<JsonResult> ChangeStatus(long idpedido, int status, string taxaservico = null)
        {
            try
            {
                await service.ChangeState(idpedido, status, taxaservico);
                var result = await service.GetPedidoById(idpedido);
                return Json(new { success = true, message = "", result = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ChangeStatusItem")]
        public async Task<JsonResult> ChangeStatusItem(long idpedidoitem, int status)
        {
            try
            {
                await service.ChangeStateItem(idpedidoitem, status);
                return Json(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DuplicateItem")]
        public async Task<JsonResult> DuplicateItem(long idpedidoitem)
        {
            try
            {
                var model = await serviceItem.GetItemById(idpedidoitem);
                model.Id = 0;
                model.IdStatusPedidoItem = (int)Settings.Status.PedidoItem.Solicitado;
                await serviceItem.Save(model);
                return Json(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }


}
