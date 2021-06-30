using Core.Entities;
using Core.Interfaces;
using CrossCutting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
    public class MainController : Controller
    {
        private readonly IUserSession userSession;
        private readonly IPedidoRepository serviceOrder;
        private readonly IMesaRepository serviceMesa;
        private readonly IDashBoardRepository serviceDashBoard;
        public MainController(IUserSession userSession, IPedidoRepository serviceOrder, IMesaRepository serviceMesa, IDashBoardRepository serviceDashBoard)
        {
            this.userSession = userSession;
            this.serviceOrder = serviceOrder;
            this.serviceMesa = serviceMesa;
            this.serviceDashBoard = serviceDashBoard;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ViewData["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (userSession.Role == Settings.Role.Garcon)
                    ViewBag.MeusPedidos = serviceOrder.GetMeusPedidos(userSession.Id).Result;
                else
                    ViewBag.MeusPedidos = serviceOrder.GetMeusPedidos(null).Result;

                var result = serviceMesa.GetAll().Result;
                result = result.Where(item => item.EmUso).ToList();

                ViewBag.PedidosImpressao = result;

            }
            catch
            {
                ViewData["version"] = DateTime.Now.Ticks.ToString();
                ViewBag.MeusPedidos = new List<Pedido>();
            }

            LoadGraphOrders();

            if (this.userSession.Role == CrossCutting.Settings.Role.Caixa || this.userSession.Role == CrossCutting.Settings.Role.Gerente)
            {
                LoadGraphOrdersByAtendente();
            }

            base.OnActionExecuting(filterContext);

        }

        private void LoadGraphOrders()
        {
            var pedidos = new List<GraphOrders>();
            if (userSession.Role == Settings.Role.Garcon)
                pedidos = serviceDashBoard.GetGraphOrders(userSession.Id).Result;
            else
                pedidos = serviceDashBoard.GetGraphOrders(null).Result;

            ViewBag.GraficoQuantidadePedidos = pedidos.Count();
            ViewBag.GraficoPedidosSemanal = pedidos;
        }

        private void LoadGraphOrdersByAtendente()
        {
            var pedidos = serviceDashBoard.GetGraphOrdersByAtendente().Result;
           
            ViewBag.GraficoQuantidadePedidosGarcom = pedidos.Count();
            ViewBag.GraficoPedidosSemanalGarcom = pedidos;
        }

    }
}
