using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSS.Server.Logger
{
    public class FileLogger
    {
        private readonly string _baseLogDirectory;

        public FileLogger(IConfiguration configuration)
        {
            _baseLogDirectory = configuration["LogFileSettings:BaseLogDirectory"];
        }

        public void Log(string message, string fileName = "BackgroundService.txt", string moduleName = "application")
        {
            string logFilePath = GetLogFilePath(fileName, moduleName);

            // Ensure directory exists
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Write or append to log file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        public string GetLogFilePath(string fileName, string moduleName)
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.ToString("MMMM");
            var day = DateTime.Now.Day.ToString();

            // Construct the directory and file structure with dynamic filename
            var directory = Path.Combine(_baseLogDirectory, moduleName, year, month, day);
            var logFilePath = Path.Combine(directory, fileName);

            return logFilePath;
        }

        public IEnumerable<string> GetRootFolders()
        {
            if (Directory.Exists(_baseLogDirectory))
            {
                return Directory.GetDirectories(_baseLogDirectory)
                                .Select(Path.GetFileName);
            }
            return Enumerable.Empty<string>();
        }
    }
}

