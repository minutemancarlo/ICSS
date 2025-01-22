using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentManagementController : ControllerBase
    {
        private readonly StudentRepository _studentRepository;

        public StudentManagementController(StudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<StudentModel>>> GetAllStudents()
        {
            try
            {
                
                var students = await _studentRepository.GetStudentsAsync();
                
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddStudent([FromBody] StudentModel student)
        {
            if (student == null)
            {
                return BadRequest(new { Message = "Invalid student data." });
            }

            try
            {
                var newId = await _studentRepository.InsertStudentAsync(student);
                return Ok(new { Message = "Student added successfully.", StudentId = newId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentModel student)
        {
            if (student == null || id <= 0)
            {
                return BadRequest(new { Message = "Invalid student data or ID." });
            }

            try
            {
                student.Id = id;
                await _studentRepository.UpdateStudentAsync(student);
                return Ok(new { Message = "Student updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
