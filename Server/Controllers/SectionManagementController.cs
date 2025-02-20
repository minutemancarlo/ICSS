using System;
using System.Data;
using Dapper;
using ICSS.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ICSS.Server.Repository;
using TagLib.Ape;

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


        [HttpGet("GetSections")]
        public async Task<ActionResult<IEnumerable<Sections>>> GetSections()
        {
            try
            {

                var sections = await _sectionRepository.GetSectionsWithCoursesAsync();

                return Ok(sections);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("InsertSection")]
        public async Task<IActionResult> InsertSection([FromBody] List<Sections> sections, [FromQuery] string userId)
        {
            if (sections == null || sections.Count == 0)
                return BadRequest("Invalid sections data.");

            try
            {

                foreach (var item in sections)
                {
                    await _sectionRepository.InsertSectionAsync(item, userId);
                }
                return Ok(new { Message = "Sections added successfully." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("UpdateSection")]
        public async Task<IActionResult> UpdateSection([FromBody] Sections sections, [FromQuery] string userId)
        {
            if (sections == null)
                return BadRequest("Invalid sections data.");

            try
            {
                await _sectionRepository.UpdateSectionAsync(sections, userId);
                return Ok(new { Message = "Sections updated successfully." });                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAvailableStudents")]
        public async Task<ActionResult<IEnumerable<StudentModel>>> GetAvailableStudents()
        {
            try
            {

                var students = await _sectionRepository.GetAvailableStudents();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetFilteredSection")]
        public async Task<ActionResult<SectionResponse>> GetFilteredSection([FromQuery] Sections section)
        {
            try
            {

                var sections = await _sectionRepository.GetSectionsSingleAsync(section);

                return Ok(sections);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}