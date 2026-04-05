using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LingvoGameOs.Controllers
{
    [Route("api/mvc")]
    [ApiController]
    public class ApiUserController : ControllerBase
    {
        //[HttpGet("current-user-id")]
        //public IActionResult GetCurrentUserId()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized(new { error = "User not authenticated" });
        //    }

        //    return Ok(new { userId });
        //}
    }
}
