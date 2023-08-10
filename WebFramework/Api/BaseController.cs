using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebFramework.Api
{
    [ApiController]
    //[AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    public class BaseController : ControllerBase
    {
        //public UserRepository UserRepository { get; set; } => property injection
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }

    [ApiController]
    //[AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    [Authorize(Roles = "Admin")]
    public class AdminApiBaseController : ControllerBase
    {
        //public UserRepository UserRepository { get; set; } => property injection
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }

    [Authorize(Roles = "Admin")]
    public class AdminBaseController : Controller
    {

    }
}
