using Core.Entities;
using Core.Interfaces;
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
        public MainController(IUserSession userSession, IPedidoRepository serviceOrder)
        {
            this.userSession = userSession;
            this.serviceOrder = serviceOrder;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ViewData["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
          
                ViewBag.MeusPedidos = serviceOrder.GetMeusPedidos(userSession.Id).Result;
            }
            catch
            {
                ViewData["version"] = DateTime.Now.Ticks.ToString();
                ViewBag.MeusPedidos = new List<Pedido>();
            }

            base.OnActionExecuting(filterContext);

        }
    }
}
