using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private CalculoFreteClient _calculoFreteClient;

        public HomeController(ILogger<HomeController> logger, CalculoFreteClient calculoFreteClient)
        {
            _logger = logger;
            _calculoFreteClient = calculoFreteClient;
        }

        public IActionResult Index()
        {
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

        [HttpGet]
        public async Task<IActionResult> CalcularFrete(decimal peso, string cepOrigem, string cepDestino)
        {
            return Ok(await _calculoFreteClient.ObterFrete(peso, cepOrigem, cepDestino));
        }
    }
}
