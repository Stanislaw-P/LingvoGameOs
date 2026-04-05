using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOsWebApi.Controllers
{
    [Route("api/ping")]
    [ApiController]
    [Authorize]
    public class PingController : ControllerBase
    {
        /// <summary>
        /// Метод нужен для проверки работы API и валидности личного ключа API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("pong");
        }
    }
}
