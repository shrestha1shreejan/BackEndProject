using Application.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DataingAppApi.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}
