using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroService.APIGateway
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : Controller
    {
        private static readonly HttpClient _httpClient;
        private ILogger _logger;
        public NoticeController(ILogger<NoticeController> logger)
        {
            _logger = logger;
     
        }

        [HttpGet]
        public IActionResult Get() {
            _logger.LogInformation("测试一下，不要紧张!");
            return   Ok("ok");
        } 

        //或者使用fabio进行健康检查
        //[Route("")]
        //[HttpGet]
        //public async Task<HttpResponseMessage> GetWithFabio() => await _httpClient.GetAsync("http://127.0.0.1:9998/health");
    }
}