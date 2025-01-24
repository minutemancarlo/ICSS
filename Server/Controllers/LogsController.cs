using ICSS.Server.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]    
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

        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path is required.");

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            try
            {
                var fileName = Path.GetFileName(filePath);
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }


}
