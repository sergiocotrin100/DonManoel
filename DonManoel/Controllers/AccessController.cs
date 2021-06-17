using Core.Entities;
using Core.Interfaces;
using CrossCutting;
using DonManoel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccessController : Controller
    {
        private readonly IUsuarioRepository _usuario;
        private readonly IUserSession _userSession;
        private IHttpContextAccessor _accessor;

        public AccessController(IUsuarioRepository usuario, IUserSession userSession, IHttpContextAccessor accessor)
        {
            _usuario = usuario;
            _accessor = accessor;
            _userSession = userSession;
        }
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LoginAsync(bool deslogou = false)
        {
            ViewBag.deslogou = deslogou;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginModel form)
        {

            if (ModelState.IsValid)
            {
                var usuario = await _usuario.Login(form.Usuario, form.Senha);
                if (usuario != null && usuario.Id >0)
                {
                    await _LoginAsync(usuario);
                }
                else
                {
                    ModelState.AddModelError("", "Dados de acesso inválidos.");
                    return View(form);
                }

                return LocalRedirect("/Home");
            }

            return View(form);

        }

        private async Task _LoginAsync(Usuario usuario)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim("Id", usuario.Id.ToString()),
                        new Claim("Nome", usuario.Nome),
                        new Claim("Login", usuario.Login),
                        new Claim("Email", usuario.Email)
                    };

            claims.Add(new Claim(ClaimTypes.Role, usuario.Role));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3),
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewData["Deslogou"] = 1;

            RedirectToActionResult redirectResult = new RedirectToActionResult("LoginAsync", "Access", new { deslogou = true });
            return redirectResult;
        }
    }
}
