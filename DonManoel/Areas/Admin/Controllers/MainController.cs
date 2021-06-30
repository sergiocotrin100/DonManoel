using Core.Entities;
using Core.Interfaces;
using CrossCutting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DonManoel.Areas.Admin.Controllers
{
 
    public class MainController : Controller
    {
        private readonly IUserSession userSession;
        private readonly IPedidoRepository serviceOrder;
        private readonly IMesaRepository serviceMesa;
        public MainController(IUserSession userSession, IPedidoRepository serviceOrder, IMesaRepository serviceMesa)
        {
            this.userSession = userSession;
            this.serviceOrder = serviceOrder;
            this.serviceMesa = serviceMesa;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ViewData["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if(userSession.Role == Settings.Role.Garcon)
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

            base.OnActionExecuting(filterContext);

        }
    }
}
