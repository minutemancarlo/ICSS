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


        [HttpGet("GetReportScheduleById/{Id}")]
        public async Task<IActionResult> GetReportScheduleById(int? Id)
        {
            try
            {
                var dt = new DataTable();
                dt = await _scheduleService.GetScheduleSingleByIdAsync(Id);

                string mimeType = "";
                int extenstion = 1;
                //var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports\\Individual", "IndividualSchedule.rdlc");
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Reports", "Report1.rdlc");

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
                localReport.AddDataSource(dataSetName: "IndividualScheduleDataset", dt);
                var result = localReport.Execute(RenderType.Pdf);

                var pdfStream = new MemoryStream(result.MainStream);
                var timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
                var fileName = $"IndividualSchedule_{timestamp}.pdf";
                mimeType = "application/pdf";

                return new FileStreamResult(pdfStream, mimeType)
                {
                    FileDownloadName = fileName
                };

            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


    }
}
