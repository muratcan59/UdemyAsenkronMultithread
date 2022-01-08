using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetContentAsync(CancellationToken token)
        {
            try
            {
                _logger.LogInformation("İstek Başladı");
                #region Cancellation Token asenkron
                //await Task.Delay(5000);
                //var myTask = new HttpClient().GetStringAsync("https://www.google.com");

                //var data = await myTask;
                //_logger.LogInformation("İstek Bitti");
                //return Ok(data);
                #endregion

                #region Cancellation Token senkron
                Enumerable.Range(1, 10).ToList().ForEach(x =>
                {
                    Thread.Sleep(1000);

                    token.ThrowIfCancellationRequested();
                });

                _logger.LogInformation("İstek Bitti");

                return Ok("İşler Bitti");
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogInformation("İstek İptal Edildi:" + ex.Message);
                return BadRequest();
            }
        }
    }
}
