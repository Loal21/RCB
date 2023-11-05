using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using CRUD_Imagenes.Models;
using CRUD_Imagenes.Recursos;
using CRUD_Imagenes.Servicios.Contrato;

namespace CRUD_Imagenes.Controllers
{
    public class InicioController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        public InicioController(IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo)
        {
            modelo.Contraseña = Utilidades.EncriptarClave(modelo.Contraseña);


            Usuario usuario_creado = await _usuarioServicio.SaveUsuario(modelo);

            if (usuario_creado.IdUsuario > 0)
                return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string documento, string contraseña)
        {
            Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(documento, Utilidades.EncriptarClave(contraseña));

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim> {
        new Claim(ClaimTypes.Name, usuario_encontrado.Nombre)
    };

            // Split the roles from the database and add them as claims
            string roles = usuario_encontrado.Rol; // Assuming roles are stored as a comma-separated string
            if (!string.IsNullOrEmpty(roles))
            {
                string[] roleNames = roles.Split(',');
                foreach (string roleName in roleNames)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }
    }
}