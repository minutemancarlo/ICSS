using System.Data;
using Dapper;
using ICSS.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            using var multi = await connection.QueryMultipleAsync("GetSections", commandType: CommandType.StoredProcedure);

            var sections = await multi.ReadAsync<Sections>();
            var courses = await multi.ReadAsync<Course>();

            // Map Course to Sections
            var courseDict = courses.ToDictionary(c => c.CourseId);
            foreach (var section in sections)
            {
                if (section.CourseId.HasValue && courseDict.TryGetValue(section.CourseId.Value, out var course))
                {
                    section.Course = course;
                }
            }

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
            parameters.Add("@UpdatedBy", userId);

            int affectedRows = await _dbConnection.ExecuteAsync("UpdateSection", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }

        public async Task<int> GetSectionsSingleAsync(Sections sections)
        {
            var query = @"SELECT COUNT(*) FROM Sections 
                  WHERE SchoolYear = @SchoolYear 
                  AND YearLevel = @YearLevel 
                  AND CourseId = @CourseId 
                  AND IsSummer = @IsSummer";

            var parameters = new DynamicParameters();
            parameters.Add("@SchoolYear", sections.SchoolYear);
            parameters.Add("@YearLevel", sections.YearLevel);
            parameters.Add("@CourseId", sections.CourseId);
            parameters.Add("@IsSummer", sections.IsSummer);

            return await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
        }



    }
}