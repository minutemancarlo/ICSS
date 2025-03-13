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


        [HttpPost("InsertRoom")]
        public async Task<IActionResult> InsertRoom([FromBody] Rooms rooms)
        {
            try
            {
                if (rooms == null)
                {
                    return BadRequest("Invalid room data.");
                }

                var result = await _departmentRepository.InsertRoomAsync(rooms);

                return Ok(new { Message = "Room inserted successfully."});

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting room. Error: {ex.Message}");
            }
        }


        [HttpPut("UpdateRoom")]
        public async Task<IActionResult> UpdateRoom([FromBody] Rooms rooms)
        {
            try
            {
                if (rooms == null || rooms.RoomId <= 0)
                {
                    return BadRequest("Invalid room data.");
                }

                var result = await _departmentRepository.UpdateRoomAsync(rooms);

                return Ok(new { Message = "Room updated successfully.", RowsAffected = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating room.Error: {ex.Message}");
            }


        }


        [HttpGet("GetAllRooms")]
        public async Task<ActionResult<IEnumerable<Rooms>>> GetAllRooms()
        {
            try
            {

                var rooms = await _departmentRepository.GetRoomsAsync();

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
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

                var departmentId = departmentMembers.First().Departments!.DepartmentId;
                var currentMembers = await _departmentRepository.GetDepartmentMembers(departmentId);

                var newMembers = departmentMembers.Select(m => m.FacultyModel!.FacultyId).ToList();
                var existingMembers = currentMembers.Select(m => m.FacultyId).ToList();

                var membersToDelete = existingMembers
                                      .Where(em => !newMembers.Contains(em))
                                      .Select(id => new DepartmentMember
                                      {
                                          FacultyModel = new FacultyModel { FacultyId = id },
                                          Departments = new Departments { DepartmentId = departmentId }
                                      })
                                      .ToList();

                if (membersToDelete.Any())
                    await _departmentRepository.SaveDepartmentMembersAsync(membersToDelete, "delete");

                var membersToInsert = departmentMembers
                                        .Where(m => !existingMembers.Contains(m.FacultyModel!.FacultyId))
                                        .ToList();

                if (membersToInsert.Any())
                    await _departmentRepository.SaveDepartmentMembersAsync(membersToInsert, "insert");

                return Ok(new { Message = "Department members saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving department members. Error: {ex.Message}");
            }
        }

        [HttpGet("GetDashboardStatistics")]
        public async Task<ActionResult<IEnumerable<int>>> GetDashboardStatistics([FromQuery]int? departmentId=null)
        {
            try
            {

                var result = await _departmentRepository.GetDepartmentStatisticsAsync(departmentId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
