using AuralizeBlazor.Options;
using ICSS.Client.Pages.Admin;
using ICSS.Server.Repository;
using ICSS.Shared;
using System.Linq;
using Status = ICSS.Shared.TaskStatus;
namespace ICSS.Server.HostedServices
{
    public class ScheduleProcessor : BackgroundService
    {
        private readonly ScheduleRepository _scheduleRepository;
        private readonly FacultyRepository _facultyRepository;
        private readonly CourseAndSubjectRepository _courseAndSubjectRepository;
        private readonly DepartmentRepository _departmentRepository;

        public ScheduleProcessor(ScheduleRepository scheduleRepository, FacultyRepository facultyRepository, CourseAndSubjectRepository courseAndSubjectRepository, DepartmentRepository departmentRepository)
        {
            _scheduleRepository = scheduleRepository;
            _facultyRepository = facultyRepository;
            _courseAndSubjectRepository = courseAndSubjectRepository;
            _departmentRepository = departmentRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var availableRooms = new List<Rooms>();
                var availableFaculty = new List<FacultyModel>();
                var scheduledClasses = new List<ScheduleTimeSlot>();
                var random = new Random();

                try
                {
                    var requests = await _scheduleRepository.GetScheduleListAsync();
                    var activeRequests = requests
                        .Where(r => r.IsActive && r.TaskStatus == Status.On_Queue)
                        .OrderBy(r => r.Semester)
                        .ThenBy(r => r.YearLevel)
                        .ThenBy(r => r.Section?.SectionId)
                        .ToList();

                    foreach (var request in activeRequests)
                    {
                        _ = await _scheduleRepository.UpdateScheduleStatus(request.ScheduleId, Status.Processing);
                        var subjects = (await _scheduleRepository.GetSubjectForProcess(request)).ToList();
                        scheduledClasses = (await _scheduleRepository.GetScheduleByIdAsync(null, request.Departments?.DepartmentId)).ToList();
                        await ProcessSchedules(request.ScheduleId, subjects, availableRooms, availableFaculty, scheduledClasses, random, request.Departments?.DepartmentId);

                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task ProcessSchedules(int? scheduleId, List<Subjects> subjects, List<Rooms> availableRooms, List<FacultyModel> availableFaculty, List<ScheduleTimeSlot> scheduledClasses, Random random, int? departmentId)
        {
            subjects = subjects.OrderBy(s => random.Next()).ToList();
            TimeSpan currentStartTime = GetRandomStartTime(random);
            TimeSpan currentLabStartTime = TimeSpan.FromHours(random.Next(8, 12));
            foreach (var subject in subjects)
            {
                availableFaculty = (await _scheduleRepository.GetFacultyForProcessByDepartment(subject.DepartmentId)).ToList();
                availableRooms = (await _scheduleRepository.GetAvailableRoomsByDepartment(departmentId)).ToList();
                if (subject.IsSaturdayClass)
                {
                    await AssignSchedule(scheduleId, subject, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), null, DayOfWeek.Saturday, null, scheduledClasses);
                    continue;
                }

                if (subject.Units == 3 && subject.LabHour > 0)
                {
                    currentStartTime = await ProcessLecture(scheduleId, subject, currentStartTime, new[] { DayOfWeek.Monday, DayOfWeek.Friday }, 1, availableRooms, availableFaculty, scheduledClasses, random);
                    currentLabStartTime=await ProcessLab(scheduleId, subject, currentLabStartTime, new[] { DayOfWeek.Tuesday,DayOfWeek.Thursday }, 1.5, availableRooms, availableFaculty, scheduledClasses);
                }
                else if (subject.Units == 2)
                {
                    currentLabStartTime = await ProcessLecture(scheduleId, subject, currentLabStartTime, new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 1, availableRooms, availableFaculty, scheduledClasses, random);
                }
                else if (subject.Units == 3 && subject.LabHour == 0)
                {
                    currentStartTime = await ProcessLecture(scheduleId, subject, currentStartTime, new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, 1, availableRooms, availableFaculty, scheduledClasses, random, 3);                    
                }
                
            }
        }

        private TimeSpan GetRandomStartTime(Random random)
        {
            return TimeSpan.FromHours(random.Next(8, 10));
        }

        private FacultyModel? GetAvailableFaculty(List<FacultyModel> availableFaculty, List<ScheduleTimeSlot> scheduledClasses, TimeSpan startTime, TimeSpan endTime, DayOfWeek day)
        {
            return availableFaculty.FirstOrDefault(f =>
                !scheduledClasses.Any(s =>
                    s.Faculty?.FacultyId == f.FacultyId &&
                    s.Day == day &&
                    ((s.StartTime >= startTime && s.StartTime < endTime) ||
                     (s.EndTime > startTime && s.EndTime <= endTime))
                )
            );
        }

        private Rooms? FindAvailableRoom(List<Rooms> availableRooms, List<ScheduleTimeSlot> scheduledClasses, TimeSpan startTime, TimeSpan endTime, DayOfWeek day)
        {
            return availableRooms.FirstOrDefault(r =>
                !scheduledClasses.Any(s =>
                    s.Room?.RoomId == r.RoomId &&
                    s.Day == day &&
                    ((s.StartTime >= startTime && s.StartTime < endTime) ||
                     (s.EndTime > startTime && s.EndTime <= endTime))
                )
            );
        }

        private async Task<TimeSpan> ProcessLecture(int? scheduleId, Subjects subject, TimeSpan startTime, DayOfWeek[] days, double duration, List<Rooms> availableRooms, List<FacultyModel> availableFaculty, List<ScheduleTimeSlot> scheduledClasses, Random random, int repeat = 2)
        {
            TimeSpan endTime;
            FacultyModel? faculty;
            Rooms? room;            
            availableRooms = availableRooms.Where(s => !s.IsLab).OrderBy(_ => random.Next()).ToList();

            while (true)
            {
                if (startTime >= new TimeSpan(12, 0, 0) && startTime < new TimeSpan(13, 0, 0))
                    startTime = new TimeSpan(13, 0, 0);

                duration = (startTime >= new TimeSpan(13, 0, 0) && subject.LabHour == 0 && subject.Units == 3) && days.Any(s => s == DayOfWeek.Wednesday) ? 1.5 : 1;
                faculty = GetAvailableFaculty(availableFaculty, scheduledClasses, startTime, startTime.Add(TimeSpan.FromHours(duration)), days[0]);
                room = FindAvailableRoom(availableRooms, scheduledClasses, startTime, startTime.Add(TimeSpan.FromHours(duration)), days[0]);
                endTime = startTime.Add(TimeSpan.FromHours(duration));

                if (faculty != null && room != null)
                    break;

                startTime = startTime.Add(TimeSpan.FromMinutes(30));
                if (startTime > new TimeSpan(17, 0, 0))
                    return startTime;
            }

            foreach (var day in days)
            {
                if (day == DayOfWeek.Wednesday && startTime >= new TimeSpan(12, 0, 0))
                    continue;

                if (endTime > new TimeSpan(17, 0, 0))
                    break;

                await AssignSchedule(scheduleId, subject, startTime, endTime, room, day, faculty, scheduledClasses);
            }

            return endTime;
        }

        private async Task<TimeSpan> ProcessLab(int? scheduleId, Subjects subject, TimeSpan startTime, DayOfWeek[] days, double duration, List<Rooms> availableRooms, List<FacultyModel> availableFaculty, List<ScheduleTimeSlot> scheduledClasses)
        {
            TimeSpan endTime;
            FacultyModel? faculty;
            Rooms? room;
            availableRooms = availableRooms.Where(s => s.IsLab).OrderBy(_ => new Random().Next()).ToList();

            while (true)
            {
                if (startTime >= new TimeSpan(12, 0, 0) && startTime < new TimeSpan(13, 0, 0))
                    startTime = new TimeSpan(13, 0, 0);

                faculty = GetAvailableFaculty(availableFaculty, scheduledClasses, startTime, startTime.Add(TimeSpan.FromHours(duration)), days[0]);
                room = FindAvailableRoom(availableRooms, scheduledClasses, startTime, startTime.Add(TimeSpan.FromHours(duration)), days[0]);
                endTime = startTime.Add(TimeSpan.FromHours(duration));

                if (faculty != null && room != null)
                    break;

                startTime = startTime.Add(TimeSpan.FromMinutes(30));
                if (startTime > new TimeSpan(17, 0, 0))
                    return startTime;
            }

            foreach (var day in days)
            {
                await AssignSchedule(scheduleId, subject, startTime, endTime, room, day, faculty, scheduledClasses);
            }

            return endTime;
        }



        private async Task AssignSchedule(int? scheduleId, Subjects subject, TimeSpan startTime, TimeSpan endTime, Rooms? room, DayOfWeek day, FacultyModel? faculty, List<ScheduleTimeSlot>? scheduledClasses)
        {
            var scheduleSlot = new ScheduleTimeSlot
            {
                ScheduleId = scheduleId,
                Subject = subject,
                Faculty = faculty,
                Room = room,
                Day = day,
                StartTime = startTime,
                EndTime = endTime
            };

            scheduledClasses.Add(scheduleSlot);
            _ = await _scheduleRepository.InsertScheduleTimeSlot(scheduleSlot);
            _ = await _scheduleRepository.UpdateScheduleStatus(scheduleId, Status.Success);
        }
    }


}
