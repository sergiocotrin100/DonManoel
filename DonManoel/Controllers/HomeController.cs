using Core.Interfaces;
using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DonManoel.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class HomeController : MainController
    {
        private readonly IUserSession _userSession;
        private readonly IMesaRepository _mesa;
        public HomeController(IUserSession userSession,IMesaRepository mesa)
        {
            _userSession = userSession;
            _mesa = mesa;
        }
        [HttpGet("Index")]
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
