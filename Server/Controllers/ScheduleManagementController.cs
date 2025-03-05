using Dapper;
using ICSS.Client.Pages.Admin;
using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Mvc;
using SectionsModel = ICSS.Shared.Sections;
namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScheduleManagementController : ControllerBase
    {
        private readonly ScheduleRepository _scheduleRepository;
        private readonly DepartmentRepository _departmentRepository;
        private readonly SectionRepository _sectionRepository;
        public ScheduleManagementController(ScheduleRepository scheduleRepository, DepartmentRepository departmentRepository, SectionRepository sectionRepository)
        {
            _scheduleRepository = scheduleRepository;
            _departmentRepository = departmentRepository;
            _sectionRepository = sectionRepository;
        }


        [HttpGet("GetScheduleList")]
        public async Task<ActionResult<IEnumerable<ScheduleRequest>>> GetScheduleList()
        {
            try
            {

                var schedules = await _scheduleRepository.GetScheduleListAsync();

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetScheduleById/{Id}")]
        public async Task<ActionResult<IEnumerable<ScheduleTimeSlot>>> GetScheduleById(int? Id)
        {
            try
            {
                var schedules= await _scheduleRepository.GetScheduleListAsync();
                var departmentId = schedules.Where(s => s.ScheduleId == Id).Select(x => x.Departments?.DepartmentId).FirstOrDefault();
                var schedulesSingle = await _scheduleRepository.GetScheduleByIdAsync(Id,departmentId);

                return Ok(schedulesSingle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost("InsertScheduleRequest")]
        public async Task<IActionResult> InsertScheduleRequest([FromBody] ScheduleRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Invalid request data.");
                }

                var sections = await _sectionRepository.GetSectionsWithCoursesAsync();

                if (sections == null || !sections.Any())
                {
                    return BadRequest("No sections data.");
                }

                var filteredSections = sections.Where(s => s.Course != null && s.Course.CourseId == request.Course?.CourseId).ToList();

                if (!filteredSections.Any())
                {
                    return BadRequest("No sections in the selected Course.");
                }

                foreach (var item in filteredSections)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var payload = new ScheduleRequest
                        {
                            Course = request.Course != null ? new Course { CourseId = request.Course.CourseId } : null,
                            YearLevel = item.YearLevel,
                            Section = item.SectionId != null ? new SectionsModel { SectionId = item.SectionId } : null,
                            Semester = i == 0 ? Semester.First_Semester : Semester.Second_Semester,
                            Departments = request.Course != null ? new Departments { DepartmentId = request.Course.Departments?.DepartmentId } : null,
                            IsActive = true,
                            CreatedBy = request.CreatedBy
                        };

                        _ = await _scheduleRepository.InsertScheduleRequestAsync(payload);
                    }
                }

                return Ok(new { Message = "Successfully inserted schedule requests." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting schedule requests. Error: {ex.Message}");
            }
        }


    }
}
