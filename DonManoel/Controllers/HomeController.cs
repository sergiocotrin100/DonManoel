using Core.Interfaces;
using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DonManoel.Controllers
{
    [Authorize]
 
    public class HomeController : MainController
    {
        private readonly IUserSession _userSession;
        private readonly IMesaRepository _mesa;
        private readonly IPedidoRepository service;
        public HomeController(IUserSession userSession,IMesaRepository mesa, IPedidoRepository service) : base(userSession, service)
        {
            _userSession = userSession;
            _mesa = mesa;
            this.service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = _mesa.GetAll().Result;
            return View(result);
        }

        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
