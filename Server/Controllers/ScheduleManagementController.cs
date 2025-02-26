using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScheduleManagementController : ControllerBase
    {
        private readonly ScheduleRepository _scheduleRepository;
        public ScheduleManagementController(ScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
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
    }
}
