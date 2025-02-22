namespace ICSS.Server.HostedServices
{
    public class ScheduleGenerator : BackgroundService
    {
        private readonly string BaseLogDirectory;

        public ScheduleGenerator(IConfiguration configuration)
        {
            BaseLogDirectory = configuration["LogFileSettings:BaseLogDirectory"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

            }
        }


        private async Task AppendLog(string logPath, string message)
        {
            var logMessage = $"{DateTime.Now}: {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, logMessage);
        }
    }

}
