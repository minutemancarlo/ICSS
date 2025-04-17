using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseAndSubjectController : ControllerBase
    {
        private readonly CourseAndSubjectRepository _courseAndSubjectRepository;

        public CourseAndSubjectController(CourseAndSubjectRepository courseAndSubjectRepository)
        {
            _courseAndSubjectRepository = courseAndSubjectRepository;
        }


        [HttpPost("InsertCourse")]
        public async Task<IActionResult> InsertCourse([FromBody] Course course)
        {
            try
            {
                if (course == null)
                {
                    return BadRequest("Invalid course data.");
                }

                var result = await _courseAndSubjectRepository.InsertCourseAsync(course);

                return Ok(new { Message = "Course inserted successfully.", CourseId = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting the course. Error: {ex.Message}");
            }


        }

        [HttpPut("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromBody] Course course)
        {
            try
            {
                if (course == null || course.CourseId <= 0)
                {
                    return BadRequest("Invalid course data.");
                }

                var result = await _courseAndSubjectRepository.UpdateCourseAsync(course);

                return Ok(new { Message = "Course updated successfully.", RowsAffected = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the course.Error: {ex.Message}");
            }


        }

        [HttpGet("GetAllCourses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            try
            {

                var courses = await _courseAndSubjectRepository.GetCoursesAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost("InsertSubject")]
        public async Task<IActionResult> InsertSubject([FromBody] Subjects subject)
        {
            try
            {
                if (subject == null)
                {
                    return BadRequest("Invalid subject data.");
                }

                var result = await _courseAndSubjectRepository.InsertUpdateSubjectAsync(subject);

                return Ok(new { Message = "Subject inserted successfully.", CourseId = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting the subject. Error: {ex.Message}");
            }
        }



        [HttpPut("UpdateSubject")]
        public async Task<IActionResult> UpdateSubject([FromBody] Subjects subject)
        {
            try
            {
                if (subject == null || subject.SubjectId<= 0)
                {
                    return BadRequest("Invalid subject data.");
                }

                var result = await _courseAndSubjectRepository.InsertUpdateSubjectAsync(subject);

                return Ok(new { Message = "Subject updated successfully.", RowsAffected = result });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the subject.Error: {ex.Message}");
            }


        }

        [HttpGet("GetAllSubjects")]
        public async Task<ActionResult<IEnumerable<Subjects>>> GetAllSubjects()
        {
            try
            {

                var subjects = await _courseAndSubjectRepository.GetSubjectsAsync();

                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("GetAvailableSubjectsForAssignment")]
        public async Task<ActionResult<IEnumerable<Subjects>>> GetAvailableSubjectsForAssignment([FromQuery] int facultyId)
        {
            try
            {
                var subjects = await _courseAndSubjectRepository.GetSubjectsAvailableForAssignmentAsync(facultyId);
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetSubjectsAssigned")]
        public async Task<ActionResult<IEnumerable<Subjects>>> GetSubjectsAssigned([FromQuery] int facultyId)
        {
            try
            {
                var subjects = await _courseAndSubjectRepository.GetSubjectsAssignedAsync(facultyId);
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("SaveAssignedSubjects")]
        public async Task<IActionResult> SaveAssignedSubjects([FromBody] List<SubjectsAssignment> assignedSubjects)
        {
            try
            {
                if (assignedSubjects == null || !assignedSubjects.Any())
                {
                    return BadRequest("Invalid subject assignment data or no subjects provided.");
                }

                var facultyId = (int)assignedSubjects.First().FacultyId;
                var currentSubjects = await _courseAndSubjectRepository.GetSubjectsAssignedAsync(facultyId);

                var newSubjectIds = assignedSubjects.Select(s => s.SubjectId).ToList();
                var existingSubjectIds = currentSubjects.Select(s => s.SubjectId).ToList();

                var subjectsToDelete = existingSubjectIds
                    .Where(existing => !newSubjectIds.Contains(existing))
                    .Select(subjectId => new SubjectsAssignment
                    {
                        FacultyId = facultyId,
                        SubjectId = subjectId
                    })
                    .ToList();

                if (subjectsToDelete.Any())
                    await _courseAndSubjectRepository.SaveAssignedSubjectsAsync(subjectsToDelete, "delete");

                var subjectsToInsert = assignedSubjects
                    .Where(s => !existingSubjectIds.Contains(s.SubjectId))
                    .ToList();

                if (subjectsToInsert.Any())
                    await _courseAndSubjectRepository.SaveAssignedSubjectsAsync(subjectsToInsert, "insert");

                return Ok(new { Message = "Subjects assigned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while assigning subjects. Error: {ex.Message}");
            }
        }


    }
}
