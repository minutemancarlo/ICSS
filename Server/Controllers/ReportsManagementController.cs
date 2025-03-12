using AspNetCore.Reporting;
using ICSS.Server.Services;
using ICSS.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace ICSS.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsManagementController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReportsManagementController(ScheduleService scheduleService, IWebHostEnvironment webHostEnvironment)
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _scheduleService = scheduleService;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet("GetReportScheduleById")]
        public async Task<IActionResult> GetReportScheduleById([FromQuery] int? Id = null, [FromQuery] int? departmentId = null)
        {
            try
            {
                var dt = new DataTable();
                dt = await _scheduleService.GetScheduleSingleByIdAsync(Id, departmentId);

                string mimeType = "";
                                                
                var path = Path.Combine(_webHostEnvironment.WebRootPath,$"Reports\\{(Id == null ? "Multiple" : "Individual")}",$"{(Id == null ? "MultipleSchedule": "IndividualSchedule") }.rdlc");                

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);                
                    
                localReport.AddDataSource(dataSetName: Id == null?"MultipleScheduleDataset":"IndividualScheduleDataset", dt);
                
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Schedule_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetReportScheduleByFacultyId")]
        public async Task<IActionResult> GetReportScheduleByFacultyId([FromQuery] int? facultyId = null)
        {
            try
            {
                var dt = new DataTable();
                dt = await _scheduleService.GetScheduleByFacultyIdAsync(facultyId);

                string mimeType = "";

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports\\Faculty", "FacultyWorkload.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);

                localReport.AddDataSource(dataSetName: "FacultyWorkloadDataset", dt);

                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Schedule_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("GetReportScheduleByRoomId")]
        public async Task<IActionResult> GetReportScheduleByRoomId([FromQuery] int? roomId = null)
        {
            try
            {
                var dt = new DataTable();
                dt = await _scheduleService.GetScheduleByRoomIdAsync(roomId);

                string mimeType = "";

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports\\Rooms", "RoomsSchedule.rdlc");

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("File not found.");
                    return NotFound("Report file not found.");
                }
                else
                {
                    Console.WriteLine("File exists");
                }
                LocalReport localReport = new(path);

                localReport.AddDataSource(dataSetName: "RoomsScheduleDataset", dt);

                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"Schedule_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


    }
}
