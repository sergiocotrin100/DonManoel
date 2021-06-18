using Core.Interfaces;
using Core.Entities;
using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DonManoel.Controllers
{
    public class SalesController : MainController
    {
        private readonly IUserSession _userSession;
        private readonly IPedidoRepository service;
        public SalesController(IUserSession userSession, IPedidoRepository service)
        {
            _userSession = userSession;
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Sales/{idmesa}/{idorder?}")]
        [HttpGet]
        public IActionResult Order(long idmesa, long? idorder)
        {
            Pedido model = new Pedido();
            model.IdMesa = idmesa;
            if (idorder.HasValue && idorder.Value > 0)
            {
                model = service.GetPedidoById(idorder.Value).Result;
            }

            ViewBag.Categorias = service.GetCategorias().Result;

            return View(model);
        }


        [Route("Sales/kitchen")]
        [HttpGet]
        public IActionResult kitchen()
        {
            var pedidos = service.GetPedidosCozinha();
            return View(pedidos);
        }

        [HttpGet]
        public IActionResult Tracking()
        {
            return View();
        }

    }
}
