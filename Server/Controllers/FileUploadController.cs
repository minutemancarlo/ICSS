using Microsoft.AspNetCore.Mvc;
using ICSS.Shared;
using TaskStatus = ICSS.Shared.TaskStatus;
using ICSS.Server.Repository;
using ICSS.Server.Logger;
namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly string _baseLogDirectory;
        private readonly string _baseDumpDirectory;
        private TasksRepository _tasksRepository;
        private readonly FileLogger _fileLogger;
        private readonly string ModuleName;

        public FileUploadController(IConfiguration configuration,TasksRepository tasksRepository) 
        {
            _baseDumpDirectory = configuration["LogFileSettings:BaseDumpDirectory"];
            _tasksRepository = tasksRepository;
            _fileLogger = new FileLogger(configuration);
            ModuleName = "File Upload";
        }

        [HttpPost("SaveFile")]
        public async Task<IActionResult> SaveFile(IFormFile file, [FromForm] string createdBy)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing or empty.");

            if (string.IsNullOrEmpty(createdBy))
                return BadRequest("CreatedBy parameter is required.");

            try
            {
                var manilaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
                var manilaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, manilaTimeZone);
                var timestamp = manilaTime.ToString("MMddyyHHmmss");
                var fileName = $"StudentImport_{timestamp}{Path.GetExtension(file.FileName)}";

                var uploadPath = Path.Combine(_baseDumpDirectory, "Student Import");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var data = new Tasks
                {
                    Status = TaskStatus.On_Queue,
                    FileName = fileName,
                    LogPath = Path.Combine(_fileLogger.GetLogFilePath($"Log_{timestamp}.txt", ModuleName)),
                    CreatedBy = createdBy,
                    TaskType = TaskType.Student

                };
                _fileLogger.Log($"File {fileName} status changed to On Queue.", $"Log_{timestamp}" + ".txt", ModuleName);
                await _tasksRepository.InsertTaskAsync(data);

                return Ok(new { FilePath = filePath, CreatedBy = createdBy });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, $"Permission error: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(500, $"Directory not found: {ex.Message}");
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"File I/O error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetStudentImports")]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetAllStudents()
        {
            try
            {

                var tasks = await _tasksRepository.GetTasksAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("update-status/{taskId}")]
        public async Task<IActionResult> UpdateStatus(int? taskId, [FromBody] TaskStatus status)
        {
            if (taskId <= 0)
                return BadRequest("Invalid TaskId.");

            var success = await _tasksRepository.UpdateTaskStatusAsync(taskId, status);

            if (success)
                return Ok("Task status updated successfully.");
            else
                return NotFound("Task not found or could not update status.");
        }



    }
}
