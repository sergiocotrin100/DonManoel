using Core.Entities;
using Core.Interfaces;
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
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace DonManoel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [AllowAnonymous]

    public class AccessUserController : Controller
    {
        private readonly IUsuarioRepository _usuario;
        private readonly IUserSession _userSession;
        private readonly IEmail _email;
        private readonly IResetSenhaRepository _resetSenhaRepository;
        private IHttpContextAccessor _accessor;

        public AccessUserController(IUsuarioRepository usuario, IUserSession userSession, IHttpContextAccessor accessor, IResetSenhaRepository resetSenhaRepository, IEmail email)
        {
            _usuario = usuario;
            _accessor = accessor;
            _userSession = userSession;
            _resetSenhaRepository = resetSenhaRepository;
            _email = email;
        }

        [AllowAnonymous]
        [HttpGet("ReenterPassAsync")]
        public async Task<IActionResult> ReenterPassAsync([FromQuery] string t)
        {
            if (!String.IsNullOrEmpty(t))
            {

                var encodedToken = HttpUtility.UrlEncode(t);
               var request = await _resetSenhaRepository.GetReset(encodedToken);
                if (request != null)
                {
                    byte[] data = Convert.FromBase64String(encodedToken);
                    DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                    if (when > DateTime.UtcNow.AddHours(24))
                    {
                        ViewBag.TokenValidadeControler = false;
                        ViewBag.TokenValidade = "Token com prazo de validade expirado! Refaça o processo de solicitação de alteração de senha.";
                    }
                    else
                    {
                        ViewBag.TokenValidadeControler = true;
                        ViewBag.TokenValidade = "Token dentro do prazo de validade!";
                        var usuario = await _usuario.HasAccount(request.IdUsuario);
                        return View("ReenterPassAsync", new LoginModel() { Usuario = usuario.Login, Email = usuario.Email });

                    }
                }
                else
                {
                    ViewBag.TokenValidadeControler = false;
                    ViewBag.TokenValidade = "Token não encontrado! Informe o Administrador.";
                }
            }
            else
            {
                ViewBag.TokenValidadeControler = false;
                ViewBag.TokenValidade = "Token não nulo ou encontrado! Informe o Administrador.";
            }
            return View("ReenterPassAsync", new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost("ReenterPassAsync")]
        public async Task<IActionResult> ReenterPassAsync( LoginModel form)
        {
            var encodedToken = !string.IsNullOrEmpty(form.Token) ? HttpUtility.UrlEncode(form.Token) : string.Empty;
            if (ModelState.IsValid)
            {
                byte[] data = Convert.FromBase64String(encodedToken);
                DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                var request = await _resetSenhaRepository.GetReset(encodedToken);
                if (request != null)
                {
                    if (request.Token == encodedToken)
                    {
                        if (when < DateTime.UtcNow.AddHours(24))
                        {
                            var usuarioResquestReset = await _usuario.HasAccount(request.IdUsuario);
                            await _usuario.Update(new Usuario() { Id = usuarioResquestReset.Id, Email = usuarioResquestReset.Email, Senha = form.Senha });

                            return LocalRedirect("/AccessUser/LoginAsync");
                        }
                        else
                        {
                            ViewBag.TokenValidadeControler = false;
                            ViewBag.TokenValidade = "Token expirado! Informe o Administrador.";
                        }
                    }
                    else
                    {
                        ViewBag.TokenValidadeControler = false;
                        ViewBag.TokenValidade = "Token não válido! Informe o Administrador.";
                    }
                }
                else
                {
                    ViewBag.TokenValidadeControler = false;
                    ViewBag.TokenValidade = "Token não válido ou expirado! Informe o Administrador.";
                }
            }
            else
            {
                ViewBag.TokenValidadeControler = false;
                ViewBag.TokenValidade = "Valores informados não são validos! Informe o Administrador.";
            }
            return LocalRedirect($"/AccessUser/ReenterPassAsync?t={encodedToken}");
        }

        [AllowAnonymous]
        [HttpGet("ResetPassAsync")]
        public async Task<IActionResult> ResetPassAsync(bool deslogou = false)
        {
            ViewBag.deslogou = deslogou;
            await HttpContext.SignOutAsync("backend");

            return View("ResetPassAsync", new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost("ResetPassAsync")]
        public async Task<IActionResult> ResetPassAsync(LoginModel form)
        {
            if (!String.IsNullOrEmpty(form.Email))
            {
                var usuario = await _usuario.HasAccount(form.Email);
                if (usuario != null && usuario.Id > 0)
                {
                    var token = await _RequestResetSenha(usuario);
                    await _EnviarEmail(usuario, token);
                }
                else
                {
                    ModelState.AddModelError("", "E-mail não cadastrado.");
                    return View(form);
                }
                return LocalRedirect("/AccessUser/LoginAsync");
            }
            return View(form);
        }

        [AllowAnonymous]
        [HttpGet("LoginAsync")]
        public async Task<IActionResult> LoginAsync(bool deslogou = false)
        {
            ViewBag.deslogou = deslogou;
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("backend");

            return View("LoginAsync", new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost("LoginAsync")]
        public async Task<IActionResult> LoginAsync(LoginModel form)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _usuario.Login(form.Usuario, form.Senha);
                if (usuario != null && usuario.Id > 0)
                {
                    await _LoginAsync(usuario);
                }
                else
                {
                    ModelState.AddModelError("", "Dados de acesso inválidos.");
                    return View("LoginAsync", form);
                }

                if(usuario.Role == CrossCutting.Settings.Role.Cozinha)
                    return LocalRedirect("/Sales/kitchen");
                else
                    return LocalRedirect("/Admin/Home");
            }

            return View("LoginAsync",form);

        }

        private async Task _LoginAsync(Usuario usuario)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim("Id", usuario.Id.ToString()),
                        new Claim("Nome", usuario.Nome),
                        new Claim("Login", usuario.Login),
                        new Claim("Email", usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.Role),
                    };

            //claims.Add(new Claim(ClaimTypes.Role, usuario.Role));

            var claimsIdentity = new ClaimsIdentity(claims, "backend");
            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                "backend",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            //await HttpContext.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    new ClaimsPrincipal(claimsIdentity),
            //    authProperties);

        }

        [AllowAnonymous]
        [HttpGet("LogoutAsync")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewData["Deslogou"] = 1;

            RedirectToActionResult redirectResult = new RedirectToActionResult("LoginAsync", "AccessUser", new { deslogou = true });
            return redirectResult;
        }

        private async Task<string> _RequestResetSenha(Usuario usuario)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            await _resetSenhaRepository.Save(usuario.Id, token);
            return token;
        }

        private async Task _EnviarEmail(Usuario usuario, string token)
        {
            string assunto = $"Resetar a senha do {usuario.Nome} - Don Manoel";
            string corpo = $"E-mail para resetar senha do usuário: {usuario.Nome} <br /> {this.Request.Scheme}://{this.Request.Host}/AccessUser/ReenterPassAsync?t={token}";
            await _email.EnviarEmailAsync(usuario.Email, assunto, corpo,null);
        }
    }
}
