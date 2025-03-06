using Dapper;
using ICSS.Shared;
using System.Data;
using System.Data.Common;
using SchedStatus = ICSS.Shared.TaskStatus;
namespace ICSS.Server.Repository
{

    public class ScheduleRepository
    {
        private readonly IDbConnection _dbConnection;

        public ScheduleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<ScheduleTimeSlot>> GetScheduleByIdAsync(int? scheduleId, int? departmentId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@ScheduleId", scheduleId);
            parameter.Add("@DepartmentId", departmentId);

            var schedules = await _dbConnection.QueryAsync<ScheduleTimeSlot, Subjects, Rooms, FacultyModel, ScheduleTimeSlot>(
                "GetScheduleById",
                (schedule, subject, room, faculty) =>
                {
                    schedule.Subject = subject;
                    schedule.Room = room;
                    schedule.Faculty = faculty;
                    return schedule;
                },
                param: parameter,
                splitOn: "SubjectId, RoomId, FacultyId",
                commandType: CommandType.StoredProcedure
            );

            return schedules;
        }

        public async Task<IEnumerable<FacultyWorkload>> GetScheduleByFacultyIdAsync(int? facultyId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@FacultyId", facultyId);

            var workloads = await _dbConnection.QueryAsync<FacultyModel, Course, Sections, ScheduleTimeSlot, Subjects, Rooms, FacultyWorkload>(
                "GetSchedulesByFacultyId",
                (faculty, course, section, scheduleTimeSlot, subject, room) =>
                {
                    scheduleTimeSlot.Subject = subject;
                    scheduleTimeSlot.Room = room;

                    return new FacultyWorkload
                    {
                        Faculty = faculty,
                        Course = course,
                        Sections = section,
                        ScheduleTimeSlot = scheduleTimeSlot
                    };
                },
                param: parameter,
                splitOn: "CourseId, SectionId, TimeSlotId, SubjectId, RoomId",
                commandType: CommandType.StoredProcedure
            );

            return workloads;
        }




        public async Task<IEnumerable<ScheduleTimeSlot>> GetScheduleByUserIdAsync(string? userId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@UserId", userId);            

            var schedules = await _dbConnection.QueryAsync<ScheduleTimeSlot, Subjects, Rooms, FacultyModel, ScheduleTimeSlot>(
                "GetScheduleByUserId",
                (schedule, subject, room, faculty) =>
                {
                    schedule.Subject = subject;
                    schedule.Room = room;
                    schedule.Faculty = faculty;
                    return schedule;
                },
                param: parameter,
                splitOn: "SubjectId, RoomId, FacultyId",
                commandType: CommandType.StoredProcedure
            );

            return schedules;
        }



        public async Task<IEnumerable<ScheduleRequest>> GetScheduleListAsync()
        {
            var schedules = await _dbConnection.QueryAsync<Course, Sections, Departments, ScheduleRequest, ScheduleRequest>(
                "GetSchedules",
                (course, section, department, schedule) =>
                {
                    schedule.Course = course;
                    schedule.Section = section;
                    schedule.Departments = department;
                    return schedule;
                },
                splitOn: "CourseId,SectionId,DepartmentId,ScheduleId",
                commandType: CommandType.StoredProcedure
            );

            return schedules;
        }

        public async Task<int> InsertScheduleRequestAsync(ScheduleRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CourseId", request.Course.CourseId);
            parameters.Add("@YearLevel", request.YearLevel);
            parameters.Add("@SectionId", request.Section.SectionId);
            parameters.Add("@Semester", request.Semester);
            parameters.Add("@DepartmentId", request.Departments?.DepartmentId);
            parameters.Add("@IsActive", request.IsActive);
            parameters.Add("@Status", SchedStatus.On_Queue);
            parameters.Add("@CreatedBy", request.CreatedBy);

            return await _dbConnection.ExecuteAsync("InsertScheduleRequest", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertScheduleTimeSlot(ScheduleTimeSlot timeSlot)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ScheduleId", timeSlot.ScheduleId);
            parameters.Add("@SubjectId", timeSlot.Subject?.SubjectId);
            parameters.Add("@RoomId", timeSlot.Room?.RoomId);
            parameters.Add("@FacultyId", timeSlot.Faculty?.FacultyId);
            parameters.Add("@DayOfWeek", (int?)timeSlot.Day);
            parameters.Add("@StartTime", timeSlot.StartTime);
            parameters.Add("@EndTime", timeSlot.EndTime);

            return await _dbConnection.ExecuteAsync("InsertScheduleTimeSlot", parameters, commandType: CommandType.StoredProcedure);
        }


        public async Task<int> UpdateScheduleStatus (int? scheduleId,SchedStatus status)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ScheduleId", scheduleId);
            parameters.Add("@Status", status);
            var query = "Update ScheduleRequest SET Status = @Status Where ScheduleId = @ScheduleId";

            return await _dbConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);
        }


        public async Task<IEnumerable<Subjects>> GetSubjectForProcess(ScheduleRequest request)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@ScheduleId", request.ScheduleId);
            
            return await _dbConnection.QueryAsync<Subjects>("GetSubjectsForProcess",parameter , commandType: CommandType.StoredProcedure);
        }


        public async Task<IEnumerable<FacultyModel>> GetFacultyForProcessByDepartment(int? departmentId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@DepartmentId", departmentId);

            return await _dbConnection.QueryAsync<FacultyModel>("GetFacultyForProcessByDepartment", parameter, commandType: CommandType.StoredProcedure);
        }


        public async Task<IEnumerable<Rooms>> GetAvailableRoomsByDepartment(int? departmentId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@DepartmentId",departmentId);
            var query = @"Select * from Rooms Where DepartmentId = @DepartmentId";
            var students = await _dbConnection.QueryAsync<Rooms>(query,parameter,commandType: CommandType.Text);
            return students;
        }

        public async Task<bool> CheckFacultyAvailability(int? facultyId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", facultyId);
            parameters.Add("@DayOfWeek", (int)day);
            parameters.Add("@StartTime", startTime);
            parameters.Add("@EndTime", endTime);

            var result = await _dbConnection.ExecuteScalarAsync<int>("CheckFacultyAvailability", parameters, commandType: CommandType.StoredProcedure);
            return result == 1;
        }

        public async Task<bool> CheckRoomAvailability(int? roomId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoomId", roomId);
            parameters.Add("@DayOfWeek", (int)day);
            parameters.Add("@StartTime", startTime);
            parameters.Add("@EndTime", endTime);

            var result = await _dbConnection.ExecuteScalarAsync<int>("CheckRoomAvailability", parameters, commandType: CommandType.StoredProcedure);
            return result == 1;
        }




    }
}
