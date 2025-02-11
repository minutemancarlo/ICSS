using System;
using System.Data;
using Dapper;
using ICSS.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ICSS.Server.Repository;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SectionManagementController : ControllerBase
    {
        private readonly SectionRepository _sectionRepository;

        public SectionManagementController(SectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        [HttpPost("InsertSection")]
        public async Task<IActionResult> InsertSection([FromBody] List<Sections> sections, [FromQuery] string userId)
        {
            if (sections == null || sections.Count == 0)
                return BadRequest("Invalid sections data.");

            try
            {
                bool result = await _sectionRepository.InsertOrUpdateSectionsAsync(sections, userId);

                if (result)
                    return Ok("Sections added successfully.");
                else
                    return StatusCode(500, "Failed to add sections.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("UpdateSection")]
        public async Task<IActionResult> UpdateSection([FromBody] List<Sections> sections, [FromQuery] string userId)
        {
            if (sections == null || sections.Count == 0)
                return BadRequest("Invalid sections data.");

            try
            {
                bool result = await _sectionRepository.InsertOrUpdateSectionsAsync(sections, userId);

                if (result)
                    return Ok("Sections updated successfully.");
                else
                    return StatusCode(500, "Failed to update sections.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}