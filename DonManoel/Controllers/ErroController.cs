using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
    public class ErroController : Controller
    {
        [AllowAnonymous]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
