using DonManoel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
    [Route("[controller]")]
    public class ErroController : Controller
    {
        [AllowAnonymous]
        [HttpGet("AcessoNegado")]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("PaginaErro")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult PaginaErro()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
