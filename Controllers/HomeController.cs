using Avance.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avance.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string message = null)
        {
            string url = "http://apiservicios.ecuasolmovsa.com:3009/api/Varios/GetEmisor";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var emisores = JsonSerializer.Deserialize<IEnumerable<EmisorModel>>(data);

                    var nombresEmisores = emisores.Select(e => e.NombreEmisor); // Obtener solo los nombres de los emisores

                    ViewBag.Message = message; // Pasar el mensaje a la vista

                    return View(nombresEmisores);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al recuperar los datos";
                    return RedirectToAction("Exception");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string emisor)
        {
            try
            {
                // Validación del usuario
                if (!string.IsNullOrEmpty(username) && username.Length <= 4 && username.All(char.IsDigit))
                {
                    // El usuario cumple con los requisitos
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario debe contener máximo 4 números.";
                    return RedirectToAction("Index");
                }

                // Validación de la contraseña
                if (!string.IsNullOrEmpty(password) && password.Length <= 5)
                {
                    // La contraseña cumple con los requisitos
                }
                else
                {
                    TempData["ErrorMessage"] = "La contraseña debe contener máximo 5 caracteres.";
                    return RedirectToAction("Index");
                }

                string encodedUsername = Uri.EscapeDataString(username);
                string encodedPassword = Uri.EscapeDataString(password);
                string url = $"http://apiservicios.ecuasolmovsa.com:3009/api/Usuarios?usuario={encodedUsername}&password={encodedPassword}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var usuarios = JsonSerializer.Deserialize<IEnumerable<Usuario>>(data);

                        // Analizar la respuesta JSON y realizar las comprobaciones necesarias
                        // ...

                        return RedirectToAction("CentroCostos", "CentroCostos");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Usuario o contraseña incorrectos";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Exception()
        {
            return View();
        }
    }
}
