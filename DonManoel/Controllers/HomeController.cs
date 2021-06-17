using Core.Interfaces;
using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
   // [Authorize]
    public class HomeController : MainController
    {
        private readonly IUserSession _userSession;
        private readonly IMesaRepository _mesa;
        public HomeController(IUserSession userSession,IMesaRepository mesa)
        {
            _userSession = userSession;
            _mesa = mesa;
        }

        public IActionResult Index()
        {
            // return View(_mesa.GetAll().Result);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
