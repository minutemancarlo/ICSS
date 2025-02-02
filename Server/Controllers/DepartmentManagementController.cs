using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepartmentManagementController : ControllerBase
    {
        private readonly DepartmentRepository _departmentRepository;
        public DepartmentManagementController(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpPost("InsertDepartment")]
        public async Task<IActionResult> InsertDepartment([FromBody] Departments departments)
        {
            try
            {
                if (departments == null)
                {
                    return BadRequest("Invalid department data.");
                }

                var result = await _departmentRepository.InsertUpdateDepartmentAsync(departments);

                return Ok(new { Message = "Deparment inserted successfully.", CourseId = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting department. Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment([FromBody] Departments departments)
        {
            try
            {
                if (departments == null || departments.DepartmentId <= 0)
                {
                    return BadRequest("Invalid department data.");
                }

                var result = await _departmentRepository.InsertUpdateDepartmentAsync(departments);

                return Ok(new { Message = "Department updated successfully.", RowsAffected = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating department.Error: {ex.Message}");
            }


        }

        [HttpGet("GetAllDepartments")]
        public async Task<ActionResult<IEnumerable<Departments>>> GetAllDepartments()
        {
            try
            {

                var deparments = await _departmentRepository.GetDepartmentsAsync();

                return Ok(deparments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost("SaveDepartmentMembers")]
        public async Task<IActionResult> SaveDepartmentMembers([FromBody] List<DepartmentMember> departmentMembers)
        {
            try
            {
                if (departmentMembers == null || !departmentMembers.Any())
                {
                    return BadRequest("Invalid department members data or no members provided.");
                }

                await _departmentRepository.SaveDepartmentMembersAsync(departmentMembers);

                return Ok(new { Message = "Department members saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving department members. Error: {ex.Message}");
            }
        }
    }
}
