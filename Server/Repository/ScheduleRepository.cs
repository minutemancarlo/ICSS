using Dapper;
using ICSS.Shared;
using System.Data;
using System.Data.Common;

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







    }
}
