using Microsoft.AspNetCore.Mvc;
using ICSS.Server.Repository;
using ICSS.Shared;
namespace ICSS.Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class FacultyManagementController : ControllerBase
    {
        private readonly FacultyRepository _facultyRepository;
        public FacultyManagementController(FacultyRepository facultyRepository) 
        {
            _facultyRepository = facultyRepository;
        }


        [HttpPost("InsertFaculty")]
        public async Task<IActionResult> InsertFaculty([FromBody] FacultyModel faculty)
        {
            try
            {
                if (faculty == null)
                {
                    return BadRequest("Invalid faculty data.");
                }

                var result = await _facultyRepository.InsertUpdateFacultyAsync(faculty);

                return Ok(new { Message = "  successfully.", CourseId = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting faculty. Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateFaculty")]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyModel faculty)
        {
            try
            {
                if (faculty == null || faculty.FacultyId <= 0)
                {
                    return BadRequest("Invalid department data.");
                }

                var result = await _facultyRepository.InsertUpdateFacultyAsync(faculty);

                return Ok(new { Message = "Faculty updated successfully.", RowsAffected = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating faculty. Error: {ex.Message}");
            }


        }

        [HttpGet("GetAllFaculty")]
        public async Task<ActionResult<IEnumerable<FacultyModel>>> GetAllFaculty()
        {
            try
            {

                var faculty = await _facultyRepository.GetFacultyAsync();

                return Ok(faculty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


    }
}
