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

        [HttpPost("UpdateFacultyStatus")]
        public async Task<IActionResult> UpdateFacultyStatus([FromQuery] int facultyId)
        {
            try
            {
                var result = await _facultyRepository.UpdateFacultyStatusAsync(facultyId);

                if (result > 0)
                {
                    return Ok(new ApiResponse<int> { IsSuccess = true, Data = result, Message = "Faculty status updated successfully." });
                }
                else
                {
                    return BadRequest(new ApiResponse<int> { IsSuccess = false, Message = "Failed to update faculty status." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<int> { IsSuccess = false, Message = $"An error occurred: {ex.Message}" });
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
        public async Task<ActionResult<IEnumerable<DepartmentMember>>> GetAllFaculty()
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

        [HttpGet("GetAllFacultyAssigned/{departmentId}")]
        public async Task<ActionResult<IEnumerable<DepartmentMember>>> GetAllFacultyAssigned(int departmentId)
        {
            try
            {
                var faculty = await _facultyRepository.GetFacultyAssignedAsync(departmentId);
                return Ok(faculty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("GetFacultyNotMember")]
        public async Task<ActionResult<IEnumerable<DepartmentMember>>> GetFacultyNotMember()
        {
            try
            {

                var faculty = await _facultyRepository.GetFacultyNotMemberAsync();                
                return Ok(faculty);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


    }
}
