using Microsoft.AspNetCore.Mvc;

namespace DocuShareIndexingAPI.Controllers
{

    /**
    * @notice inherite ApiController to base controller.
    */
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }
}