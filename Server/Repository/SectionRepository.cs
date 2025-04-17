using System.Data;
using Dapper;
using ICSS.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;


namespace ICSS.Server.Repository
{
    public class SectionRepository
    {
        private readonly IDbConnection _dbConnection;

        public SectionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Sections>> GetSectionsWithCoursesAsync()
        {
            using var connection = _dbConnection;

            var sections = await connection.QueryAsync<Sections, Course, Departments, Sections>(
                "GetSections",
                (section, course, department) =>
                {
                    if (course != null)
                    {
                        section.Course = course;
                        if (department != null)
                        {
                            course.Departments = department;
                        }
                    }
                    return section;
                },
                splitOn: "CourseId,DepartmentId",
                commandType: CommandType.StoredProcedure
            );

            return sections.ToList();
        }



        public async Task<bool> InsertSectionAsync(Sections section, string userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SectionName", section.SectionName);
            parameters.Add("@IsSummer", section.IsSummer);
            parameters.Add("@YearLevel", (int)section.YearLevel);
            parameters.Add("@IsDeleted", section.IsDeleted);
            parameters.Add("@CourseId", section.CourseId);
            parameters.Add("@SchoolYear", section.SchoolYear);
            parameters.Add("@ClassSize", section.ClassSize);
            parameters.Add("@CreatedBy", userId);

            int affectedRows = await _dbConnection.ExecuteAsync("InsertSection", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }


        public async Task<bool> UpdateSectionAsync(Sections section, string userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SectionId", section.SectionId);
            parameters.Add("@SectionName", section.SectionName);
            parameters.Add("@IsSummer", section.IsSummer);
            parameters.Add("@YearLevel", (int)section.YearLevel);
            parameters.Add("@IsDeleted", section.IsDeleted);
            parameters.Add("@CourseId", section.CourseId);
            parameters.Add("@SchoolYear", section.SchoolYear);
            parameters.Add("@ClassSize", section.ClassSize);
            parameters.Add("@UpdatedBy", userId);

            int affectedRows = await _dbConnection.ExecuteAsync("UpdateSection", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }

        public async Task<SectionResponse> GetSectionsSingleAsync(Sections sections)
        {
            var query = "GetFilteredSectionsWithStudents";

            var parameters = new DynamicParameters();
            parameters.Add("@SchoolYear", sections.SchoolYear);
            parameters.Add("@YearLevel", sections.YearLevel);
            parameters.Add("@CourseId", sections.CourseId);
            parameters.Add("@SectionName", sections.SectionName);

            using (var multi = await _dbConnection.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure))
            {
                var section = multi.ReadFirstOrDefault<Sections>();
                var students = multi.Read<StudentModel>().ToList();

                var response = new SectionResponse
                {
                    Sections = section,
                    Students = students
                };

                return response;
            }
        }



        public async Task<IEnumerable<StudentModel>> GetAvailableStudents()
        {
            var query = @"SELECT A.*
                        FROM Students A 
                        LEFT JOIN SectionMember B ON A.Id = B.StudentId 
                        LEFT JOIN Sections C on B.SectionId = C.SectionId
                        WHERE B.StudentId IS NULL AND (C.IsActive = 0 OR C.IsActive IS NULL)";
            var students = await _dbConnection.QueryAsync<StudentModel>(query);
            return students;
        }

        public async Task<IEnumerable<Sections>> GetSectionByUserIdAsync(string id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@UserId", id);

            var result = await _dbConnection.QueryAsync<Sections, Course, Sections>(
                "GetSectionByUserId",
                (section, course) =>
                {
                    section.Course = course;
                    return section;
                },
                parameter,
                commandType: CommandType.StoredProcedure,
                splitOn: "CourseId"
            );

            return result;
        }



        public async Task<IEnumerable<SectionMember>> GetSectionMembers(int? sectionId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@SectionId",sectionId);
            var query = @"Select * from SectionMember Where SectionId = @SectionId";

            var members = await _dbConnection.QueryAsync<SectionMember>(query,parameter);
            return members;
        }

        public async Task UpdateSectionMemberAsync(List<SectionMember> members, string action)
        {
            var query = new StringBuilder();
            var parameters = new DynamicParameters();

            for (int i = 0; i < members.Count; i++)
            {
                if (action.Equals("insert", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
                    INSERT INTO SectionMember (StudentId, SectionId)
                    VALUES (@StudentId{i}, @SectionId{i});");

                    parameters.Add($"StudentId{i}", members[i].StudentId);
                    parameters.Add($"SectionId{i}", members[i].SectionId);
                }
                else if (action.Equals("delete", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
                    DELETE FROM SectionMember 
                    WHERE StudentId = @StudentId{i} AND SectionId = @SectionId{i};");

                    parameters.Add($"StudentId{i}", members[i].StudentId);
                    parameters.Add($"SectionId{i}", members[i].SectionId);
                }
            }

            await _dbConnection.ExecuteAsync(query.ToString(), parameters);
        }


        //public async Task<IEnumerable<SectionMember>> GetSectionScheduleFilter(Sections sections)
        //{
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@SectionId", sectionId);
        //    var query = @"Select * from SectionMember Where SectionId = @SectionId";

        //    var members = await _dbConnection.QueryAsync<SectionMember>(query, parameter);
        //    return members;
        //}


    }
}