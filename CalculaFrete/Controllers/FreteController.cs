using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalculaFrete.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreteController : ControllerBase
    {
        ILogger<FreteController> _logger;
        public FreteController(ILogger<FreteController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Calcular(decimal peso, string cepOrigem, string cepDestino)
        {

            _logger.LogInformation("Inicializando calculo");

            if (peso == 0)
                return BadRequest("Peso do objeto precisa ser maior que zero");
            if (string.IsNullOrEmpty(cepOrigem))
                return BadRequest("O Cep Origem precisa ser preenchido");
            if (string.IsNullOrEmpty(cepDestino))
                return BadRequest("O Cep Destino precisa ser preenchido");

            if(cepOrigem.Equals("37800-000"))
                return Ok(55.95);

            return Ok(250.0);
        }
    }
}
