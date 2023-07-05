using Avance.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using System.Threading.Tasks;

namespace Avance.Controllers
{
    public class CentroCostosController : Controller
    {
        //AQUI USAMOS SINGELTON
        //private readonly HttpClient httpClient;
        // public CentroCostosController()
        // {
        //    httpClient = new HttpClient();
        // }
        public async Task<IActionResult> CentroCostos()
        {
            string url = "http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosSelect";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var centroCostos = JsonSerializer.Deserialize<IEnumerable<CentroCostos>>(data);
                    return View("indexcentro", centroCostos);
                }
                else
                {
                    return View("Exception", new { message = "Error retrieving data" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PushCentroCostos(string codigo, string descripcion)
        {
            try
            {
                string url = $"http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosInsert?codigocentrocostos={codigo}&descripcioncentrocostos={descripcion}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("CentroCostos");
                    }
                    else
                    {
                        return View("Exception", new { message = "Error al insertar el centro de costos" });
                    }
                }
            }
            catch
            {
                return View("Exception", new { message = "Error al insertar el centro de costos" });
            }
        }

        public IActionResult AddCentroDeCostos()
        {
            return View("IndexAgregar");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string descripcion)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosUpdate?codigocentrocostos={id}&descripcioncentrocostos={descripcion}");
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return View("Exceptiondentro", new { message = "Error al editar el centro de costos" });
            }

            return await CentroCostos();
        }

        public async Task<IActionResult> EditCentroCostos(string id)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosSelect");
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IEnumerable<CentroCostos>>(content);

            foreach (var datos in data)
            {
                if (datos.Codigo == int.Parse(id))
                {
                    return View("IndexEditar", datos);
                }
            }

            return await CentroCostos();
        }
        [HttpPost]
        public async Task<IActionResult> ElimAtributo(string id, string descripcion)
        {
            if (Request.Method == "POST")
            {
                try
                {
                    string url = $"http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosDelete?codigocentrocostos={id}&descripcioncentrocostos={descripcion}";
                    HttpClient httpClient = new HttpClient();
                    var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    return View("Exceptiondentro", new { message = "Error al eliminar el centro de costos" });
                }

                return await CentroCostos();
            }
            else
            {
                return await CentroCostos();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Search(string descripcion)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://apiservicios.ecuasolmovsa.com:3009/api/Varios/CentroCostosSearch?descripcioncentrocostos={descripcion}");
            var data = await response.Content.ReadAsStringAsync();

            var parsedData = JsonSerializer.Deserialize<List<CentroCostos>>(data);
            if (parsedData.Count > 0)
            {
                var codigo = parsedData[0].Codigo;
                var descripcionMostrar = parsedData[0].NombreCentroCostos;

                ViewData["codigo"] = codigo;
                ViewData["NombreCentroCostos"] = descripcionMostrar;

                return View("IndexBuscar");
            }
            else
            {
                // Manejar el caso en que no se encuentren resultados.
                ViewData["codigo"] = "";
                ViewData["NombreCentroCostos"] = "";

                return View("IndexBuscar");
            }
        }




    }
}

