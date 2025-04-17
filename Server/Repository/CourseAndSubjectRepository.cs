using Dapper;
using ICSS.Client.Pages.Admin;
using ICSS.Shared;
using System.Data;
using System.Text;

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
            parameters.Add("@DepartmentId", course.Departments?.DepartmentId);
            parameters.Add("@CreatedBy", course.CreatedBy);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertCourse", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateCourseAsync(Course course)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CourseId", course.CourseId);
            parameters.Add("@CourseCode", course.CourseCode);
            parameters.Add("@CourseName", course.CourseName);
            parameters.Add("@DepartmentId", course.Departments?.DepartmentId);
            parameters.Add("@UpdatedBy", course.UpdatedBy);

            return await _dbConnection.ExecuteScalarAsync<int>("UpdateCourse", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            var result = await _dbConnection.QueryAsync<Course, Departments, Course>("GetCourses", (course, department) =>
            {
                course.Departments = department;
                return course;
            },
               commandType: CommandType.StoredProcedure,
               splitOn: "DepartmentId"
           );

            return result;            
        }


        public async Task<int> InsertUpdateSubjectAsync(Subjects subject)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SubjectId", subject.SubjectId, DbType.Int32);
            parameters.Add("@SubjectCode", subject.SubjectCode, DbType.String);
            parameters.Add("@SubjectName", subject.SubjectName, DbType.String);
            parameters.Add("@LectureHour", subject.LectureHour, DbType.Decimal);
            parameters.Add("@LabHour", subject.LabHour, DbType.Decimal);
            parameters.Add("@Units", subject.Units, DbType.Decimal);
            parameters.Add("@MaxStudent", subject.MaxStudent, DbType.Int32);
            parameters.Add("@IsActive", subject.IsActive, DbType.Boolean);
            parameters.Add("@DepartmentId", subject.Departments.DepartmentId, DbType.Int32);
            parameters.Add("@YearLevel", subject.YearLevel, DbType.Int32);
            parameters.Add("@Semester", subject.Semester, DbType.Int32);
            parameters.Add("@CourseId", subject.CourseId, DbType.Int32);
            parameters.Add("@IsSaturdayClass", subject.IsSaturdayClass);
            parameters.Add("@User", subject.UpdatedBy ?? subject.CreatedBy, DbType.String);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertUpdateSubject", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Subjects>> GetSubjectsAsync()
        {
            var result = await _dbConnection.QueryAsync<Subjects, Departments, Subjects>("GetSubjects",(subject, department) =>
            {
                subject.Departments = department;
                    return subject;
            },
                commandType: CommandType.StoredProcedure,
                splitOn: "DepartmentId"
            );

            return result;
        }

        public async Task<IEnumerable<Subjects>> GetSubjectsAvailableForAssignmentAsync(int facultyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", facultyId);

            return await _dbConnection.QueryAsync<Subjects>("GetAvailableSubjectsForAssignment", parameters, commandType: CommandType.StoredProcedure);


        }


        public async Task<IEnumerable<Subjects>> GetSubjectsAssignedAsync(int facultyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", facultyId);

            return await _dbConnection.QueryAsync<Subjects>("GetSubjectsAssigned", parameters, commandType: CommandType.StoredProcedure);


        }

        public async Task SaveAssignedSubjectsAsync(List<SubjectsAssignment> subjects, string action)
        {
            var query = new StringBuilder();
            var parameters = new DynamicParameters();

            for (int i = 0; i < subjects.Count; i++)
            {
                if (action.Equals("insert", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
            INSERT INTO SubjectAssigned (FacultyId, SubjectId)
            VALUES (@FacultyId{i}, @SubjectId{i});");

                    parameters.Add($"FacultyId{i}", subjects[i].FacultyId);
                    parameters.Add($"SubjectId{i}", subjects[i].SubjectId);
                }
                else if (action.Equals("delete", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
            DELETE FROM SubjectAssigned 
            WHERE FacultyId = @FacultyId{i} AND SubjectId = @SubjectId{i};");

                    parameters.Add($"FacultyId{i}", subjects[i].FacultyId);
                    parameters.Add($"SubjectId{i}", subjects[i].SubjectId);
                }
            }

            await _dbConnection.ExecuteAsync(query.ToString(), parameters);
        }





    }
}
