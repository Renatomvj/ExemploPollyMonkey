using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Contrib.Simmy;

namespace Ecommerce
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            //Injeta uma Exception 
            //Geração de uma mensagem simulando o erro HTTP do tipo 503
            var faultMessage = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("Erro HTTP 503: Simulação")
            };

            // Configuração de uma falha a ser simulada via Simmy
            var faultPolicy = MonkeyPolicy.InjectFaultAsync(faultMessage, 0.5, () => true);
         

            // Injeta latência
            var chaosPolicyLatency = MonkeyPolicy.InjectLatencyAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15),0.5, () => true);

            // Cria uma política de timeout
            IAsyncPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

            var retryPolice = Policy.
                HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).
                Or<Exception>().
                WaitAndRetryAsync(5,
                    //retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    retryAttemp => TimeSpan.FromSeconds(1),
                    onRetry: (exception, timespan, retryCount, context) =>
                  {
                      string msg = $"Retentativas:  {retryCount}";
                      Console.Out.WriteLine(msg);
                      //Console.Out.WriteLine("Erro =" + exception.Exception.Message);
                      Console.Out.WriteLine(context.PolicyKey);
                  });


            var policyWrap = Policy.WrapAsync(retryPolice, chaosPolicyLatency, faultPolicy);


            services.AddHttpClient<CalculoFreteClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration["BaseUrl"]);
            }).AddPolicyHandler(policyWrap);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
