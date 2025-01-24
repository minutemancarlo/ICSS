using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ICSS.Server.Logger;
using ICSS.Server.Repository;
using ICSS.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using TaskStatus = ICSS.Shared.TaskStatus;
namespace ICSS.Server.HostedServices
{
    public class StudentInfoUploader : BackgroundService
    {
        private readonly TasksRepository _tasksRepository;
        private readonly StudentRepository _studentRepository;
        private readonly string _baseDumpDirectory;
        private readonly FileLogger _logger;

        public StudentInfoUploader(TasksRepository taskRepository,StudentRepository studentRepository ,IConfiguration configuration, FileLogger logger)
        {
            _studentRepository = studentRepository;
            _tasksRepository = taskRepository;
            _baseDumpDirectory = configuration["LogFileSettings:BaseDumpDirectory"];
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var onQueueTasks = await _tasksRepository.GetOnQueueTasksAsync();

                foreach (var task in onQueueTasks)
                {
                    try
                    {
                        var filePath = Path.Combine(_baseDumpDirectory, "Student Import", task.FileName);
                        if (!File.Exists(filePath))
                        {
                            await AppendLog(task.LogPath, $"File not found: {filePath}");
                            await AppendLog(task.LogPath, $"File {task.FileName} status changed to Failed");
                            await _tasksRepository.UpdateTaskStatusAsync(task.TaskId, TaskStatus.Failed);
                            continue;
                        }

                        int totalRows = 0, insertedRows = 0;
                        var logBuilder = new StringBuilder();
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null)
                            {
                                await AppendLog(task.LogPath, $"No worksheet found in file: {filePath}");
                                await AppendLog(task.LogPath, $"File {task.FileName} status changed to Failed");
                                await _tasksRepository.UpdateTaskStatusAsync(task.TaskId, TaskStatus.Failed);
                                continue;
                            }

                            if (!ValidateTemplateHeader(worksheet, task.LogPath))
                            {
                                await _tasksRepository.UpdateTaskStatusAsync(task.TaskId, TaskStatus.Failed);
                                await AppendLog(task.LogPath, $"File {task.FileName} status changed to Failed");
                                continue;
                            }

                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                totalRows++;
                                try
                                {
                                    var idNumber = worksheet.Cells[row, 1].Text.Trim();
                                    if (!int.TryParse(idNumber, out var parsedId) || parsedId <= 0)
                                    {
                                        await AppendLog(task.LogPath, $"Invalid or missing IDNumber in row {row}.");
                                        continue;
                                    }

                                    var name = worksheet.Cells[row, 2].Text.Trim();
                                    var email = worksheet.Cells[row, 3].Text.Trim();

                                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                                    {
                                        logBuilder.AppendLine($"Invalid data at row {row}: {idNumber}, {name}, {email}");
                                        continue;
                                    }

                                    var exists = await _tasksRepository.VerifyStudentExistenceAsync(idNumber);
                                    if (exists)
                                    {
                                        logBuilder.AppendLine($"ID {idNumber} already exists. Skipping row {row}.");
                                        continue;
                                    }

                                    var student = new StudentModel
                                    {
                                        IdNumber = idNumber,
                                        Name = name,
                                        Email = email,
                                        CreatedBy = task.CreatedBy,
                                        IsActive = true
                                    };

                                    await _studentRepository.InsertStudentAsync(student);
                                    insertedRows++;
                                }
                                catch (Exception rowEx)
                                {
                                    await AppendLog(task.LogPath, $"Error processing row {row}: {rowEx.Message}");
                                }
                            }
                        }

                        await _tasksRepository.UpdateTaskStatusAsync(task.TaskId, TaskStatus.Success);
                        logBuilder.AppendLine($"{insertedRows} out of {totalRows} students inserted.");
                        await AppendLog(task.LogPath, logBuilder.ToString());
                    }
                    catch (Exception taskEx)
                    {
                        await AppendLog(task.LogPath, $"Error processing task {task.TaskId}: {taskEx.Message}");
                        await _tasksRepository.UpdateTaskStatusAsync(task.TaskId, TaskStatus.Failed);
                    }
                }

                await Task.Delay(10000, stoppingToken);
            }
        }


        private bool ValidateTemplateHeader(ExcelWorksheet worksheet, string logPath)
        {
            var expectedHeaders = new[] { "Student ID", "Name", "Email" };
            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (!worksheet.Cells[1, col].Text.Trim().Equals(expectedHeaders[col - 1], StringComparison.OrdinalIgnoreCase))
                {
                    AppendLog(logPath, "Invalid Excel template. Expected headers: 'Student ID', 'Name', 'Email'.").Wait();
                    return false;
                }
            }
            return true;
        }

        private async Task AppendLog(string logPath, string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, logMessage);
        }
    }
}
