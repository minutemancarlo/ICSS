using ICSS.Server.Repository;
using ICSS.Shared;
using Status = ICSS.Shared.TaskStatus;
namespace ICSS.Server.HostedServices
{
    public class ScheduleGenerator : BackgroundService
    {
        private readonly string BaseLogDirectory;
        private readonly ScheduleRepository _scheduleRepository;
        private readonly FacultyRepository _facultyRepository;
        private readonly CourseAndSubjectRepository _courseAndSubjectRepository;
        private readonly DepartmentRepository _departmentRepository;

        public ScheduleGenerator(IConfiguration configuration, ScheduleRepository scheduleRepository, FacultyRepository facultyRepository, CourseAndSubjectRepository courseAndSubjectRepository, DepartmentRepository departmentRepository)
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
                try
                {


                    var requests = await _scheduleRepository.GetScheduleListAsync();
                    var activeRequests = requests.Where(r => r.IsActive && r.TaskStatus == Status.On_Queue).ToList();

                    foreach (var request in activeRequests)
                    {
                        var subjectsForProcess = await _scheduleRepository.GetSubjectForProcess(request);

                        var scheduleTimeSlots = new List<ScheduleTimeSlot>();

                        var sortedSubjects = subjectsForProcess
                            .OrderByDescending(s => s.LabHour > 0)
                            .ToList();

                        foreach (var subject in sortedSubjects)
                        {
                            var availableFaculty = await _scheduleRepository.GetFacultyForProcessByDepartment(subject.DepartmentId);
                            var availableRooms = await _scheduleRepository.GetAvailableRoomsByDepartment(subject.DepartmentId);

                            if (subject.LabHour > 0)
                            {
                                var labDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday };

                                foreach (var day in labDays)
                                {
                                    foreach (var faculty in availableFaculty)
                                    {
                                        foreach (var room in availableRooms)
                                        {
                                            var lastEndTime = scheduleTimeSlots
                                                .Where(s => s.Day == day && s.Room == room)
                                                .OrderByDescending(s => s.EndTime)
                                                .FirstOrDefault()?.EndTime ?? new TimeSpan(8, 0, 0);

                                            var startTime = lastEndTime;
                                            if (startTime == new TimeSpan(12, 0, 0))
                                            {
                                                startTime = new TimeSpan(13, 0, 0);                                                ;
                                            }
                                            var duration = TimeSpan.FromHours(1.5);
                                            var endTime = startTime + duration;
                                           
                                            var isFacultyAvailable = await _scheduleRepository.CheckFacultyAvailability(faculty.FacultyId, day, startTime, endTime);
                                            var isRoomAvailable = await _scheduleRepository.CheckRoomAvailability(room.RoomId, day, startTime, endTime);
                                            var isSectionConflict = scheduleTimeSlots.Any(s =>
                                                s.Day == day &&
                                                s.Room.RoomId == room.RoomId &&
                                                (
                                                    (startTime >= s.StartTime && startTime < s.EndTime) ||
                                                    (endTime > s.StartTime && endTime <= s.EndTime) ||
                                                    (startTime <= s.StartTime && endTime >= s.EndTime)
                                                )
                                            );

                                            if (isFacultyAvailable && isRoomAvailable && !isSectionConflict)
                                            {
                                                var timeslotSchedule = new ScheduleTimeSlot
                                                {
                                                    Subject = subject,
                                                    Faculty = faculty,
                                                    Room = room,
                                                    Day = day,
                                                    StartTime = startTime,
                                                    EndTime = endTime
                                                };
                                                scheduleTimeSlots.Add(timeslotSchedule);
                                                _ = await _scheduleRepository.InsertScheduleTimeSlot(timeslotSchedule);
                                                break;
                                            }
                                        }
                                        if (scheduleTimeSlots.Any(s => s.Subject == subject && s.Day == day))
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                var lectureDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday };

                                foreach (var day in lectureDays)
                                {
                                    if (day == DayOfWeek.Saturday && subject.IsSaturdayClass)
                                    {
                                        foreach (var faculty in availableFaculty)
                                        {
                                            foreach (var room in availableRooms)
                                            {
                                                var startTime = new TimeSpan(8, 0, 0);
                                                var endTime = new TimeSpan(12, 0, 0);

                                                var isFacultyAvailable = await _scheduleRepository.CheckFacultyAvailability(faculty.FacultyId, day, startTime, endTime);
                                                var isRoomAvailable = await _scheduleRepository.CheckRoomAvailability(room.RoomId, day, startTime, endTime);
                                                var isSectionConflict = scheduleTimeSlots.Any(s =>
                                                    s.Day == day &&
                                                    s.Room.RoomId == room.RoomId &&
                                                    (
                                                        (startTime >= s.StartTime && startTime < s.EndTime) ||
                                                        (endTime > s.StartTime && endTime <= s.EndTime) ||
                                                        (startTime <= s.StartTime && endTime >= s.EndTime)
                                                    )
                                                );

                                                if (isFacultyAvailable && isRoomAvailable && !isSectionConflict)
                                                {
                                                    var timeslotSchedule = new ScheduleTimeSlot
                                                    {
                                                        Subject = subject,
                                                        Faculty = faculty,
                                                        Room = room,
                                                        Day = day,
                                                        StartTime = startTime,
                                                        EndTime = endTime
                                                    };
                                                    scheduleTimeSlots.Add(timeslotSchedule);
                                                    //_ = await _scheduleRepository.InsertScheduleTimeSlot(timeslotSchedule);
                                                    break;
                                                }
                                            }
                                            if (scheduleTimeSlots.Any(s => s.Subject == subject && s.Day == day))
                                                break;
                                        }
                                    }
                                    else if (day != DayOfWeek.Saturday)
                                    {
                                        foreach (var faculty in availableFaculty)
                                        {
                                            foreach (var room in availableRooms)
                                            {
                                                var lastEndTime = scheduleTimeSlots
                                                    .Where(s => s.Day == day && s.Room == room)
                                                    .OrderByDescending(s => s.EndTime)
                                                    .FirstOrDefault()?.EndTime ?? new TimeSpan(7, 0, 0);

                                                var startTime = lastEndTime;
                                                if (startTime == new TimeSpan(12, 0, 0))
                                                {
                                                    startTime = new TimeSpan(13, 0, 0); ;
                                                }
                                                var duration = TimeSpan.FromHours(1);
                                                var endTime = startTime + duration;

                                                var isFacultyAvailable = await _scheduleRepository.CheckFacultyAvailability(faculty.FacultyId, day, startTime, endTime);
                                                var isRoomAvailable = await _scheduleRepository.CheckRoomAvailability(room.RoomId, day, startTime, endTime);
                                                var isSectionConflict = scheduleTimeSlots.Any(s =>
                                                    s.Day == day &&
                                                    s.Room.RoomId == room.RoomId &&
                                                    (
                                                        (startTime >= s.StartTime && startTime < s.EndTime) ||
                                                        (endTime > s.StartTime && endTime <= s.EndTime) ||
                                                        (startTime <= s.StartTime && endTime >= s.EndTime)
                                                    )
                                                );

                                                if (isFacultyAvailable && isRoomAvailable && !isSectionConflict)
                                                {
                                                    var timeslotSchedule = new ScheduleTimeSlot
                                                    {
                                                        Subject = subject,
                                                        Faculty = faculty,
                                                        Room = room,
                                                        Day = day,
                                                        StartTime = startTime,
                                                        EndTime = endTime
                                                    };
                                                    scheduleTimeSlots.Add(timeslotSchedule);
                                                    //_ = await _scheduleRepository.InsertScheduleTimeSlot(timeslotSchedule);
                                                    break;
                                                }
                                            }
                                            if (scheduleTimeSlots.Any(s => s.Subject == subject && s.Day == day))
                                                break;
                                        }
                                    }
                                }
                            }
                        }


                    }




                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }


        private async Task AppendLog(string logPath, string message)
        {
            var logMessage = $"{DateTime.Now}: {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, logMessage);
        }
    }

}
