using ICSS.Server.Repository;

namespace ICSS.Server.HostedServices
{
    public class ScheduleGenerator : BackgroundService
    {
        private readonly string BaseLogDirectory;
        private readonly ScheduleRepository _scheduleRepository;
        private readonly FacultyRepository _facultyRepository;
        private readonly CourseAndSubjectRepository _courseAndSubjectRepository;
        private readonly DepartmentRepository _departmentRepository;

        public ScheduleGenerator(IConfiguration configuration, ScheduleRepository scheduleRepository,FacultyRepository facultyRepository,CourseAndSubjectRepository courseAndSubjectRepository,DepartmentRepository departmentRepository)
        {
            BaseLogDirectory = configuration["LogFileSettings:BaseLogDirectory"];
            _scheduleRepository = scheduleRepository;
            _courseAndSubjectRepository = courseAndSubjectRepository;
            _departmentRepository = departmentRepository;
            _facultyRepository = facultyRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var requests = await _scheduleRepository.GetScheduleListAsync();
                var activeRequests = requests.Where(r => r.IsActive).ToList();
                

            }
        }


        private async Task AppendLog(string logPath, string message)
        {
            var logMessage = $"{DateTime.Now}: {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, logMessage);
        }
    }

}
