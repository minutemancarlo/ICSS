using ICSS.Server.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly FileLogger _fileLogger;

        public LogsController(FileLogger fileLogger)
        {
            _fileLogger = fileLogger;
        }

        [HttpGet("root-folders")]
        public IActionResult GetRootFolders()
        {
            var rootFolders = _fileLogger.GetRootFolders();
            return Ok(rootFolders);
        }
    }
}
