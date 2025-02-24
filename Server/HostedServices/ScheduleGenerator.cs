using ICSS.Server.Repository;

namespace ICSS.Server.HostedServices
{
    public class ScheduleGenerator : BackgroundService
    {
        private readonly string BaseLogDirectory;
        private readonly CourseAndSubjectRepository _courseAndSubjectRepository;

        public ScheduleGenerator(IConfiguration configuration, CourseAndSubjectRepository courseAndSubjectRepository)
        {
            BaseLogDirectory = configuration["LogFileSettings:BaseLogDirectory"];
            _courseAndSubjectRepository = courseAndSubjectRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var courses = _courseAndSubjectRepository.GetCoursesAsync();

            }
        }


        private async Task AppendLog(string logPath, string message)
        {
            var logMessage = $"{DateTime.Now}: {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, logMessage);
        }
    }

}
