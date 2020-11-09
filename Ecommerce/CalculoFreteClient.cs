using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce
{
    public class CalculoFreteClient
    {
        private HttpClient _httpClient;



        public CalculoFreteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ObterFrete(decimal peso, string cepOrigem, string cepDestino)
        {
            try
            {
                var resultado = await _httpClient.GetAsync($"api/Frete?peso={peso}&cepOrigem={cepOrigem}&cepDestino={cepDestino}");

                if (resultado.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<decimal>(await resultado.Content.ReadAsStringAsync());
                else
                    throw new Exception(await resultado.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
