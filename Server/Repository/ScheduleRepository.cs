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
            parameters.Add("@DepartmentId", request.Departments.DepartmentId);
            parameters.Add("@IsActive", request.IsActive);
            parameters.Add("@Status", SchedStatus.On_Queue);
            parameters.Add("@CreatedBy", request.CreatedBy);

            return await _dbConnection.ExecuteAsync("InsertScheduleRequest", parameters, commandType: CommandType.StoredProcedure);
        }


     






    }
}
