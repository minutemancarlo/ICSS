using Dapper;
using ICSS.Shared;
using System.Data;

namespace ICSS.Server.Repository
{
    public class CourseAndSubjectRepository
    {
        private readonly IDbConnection _dbConnection;

        public CourseAndSubjectRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<int> InsertCourseAsync(Course course)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CourseCode", course.CourseCode);
            parameters.Add("@CourseName", course.CourseName);            
            parameters.Add("@CreatedBy", course.CreatedBy);
                        
            return await _dbConnection.ExecuteScalarAsync<int>("InsertCourse", parameters, commandType: CommandType.StoredProcedure);
        }


    }
}
