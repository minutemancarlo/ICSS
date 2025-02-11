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

        public async Task<bool> InsertOrUpdateSectionsAsync(List<Sections> sections, string userId)
        {
            var table = new DataTable();
            table.Columns.Add("SectionId", typeof(int));
            table.Columns.Add("SectionName", typeof(string));
            table.Columns.Add("IsSummer", typeof(bool));
            table.Columns.Add("YearLevel", typeof(int));
            table.Columns.Add("IsDeleted", typeof(bool));
            table.Columns.Add("CourseId", typeof(int));

            foreach (var section in sections)
            {
                table.Rows.Add(
                    section.SectionId ?? (object)DBNull.Value,
                    section.SectionName,
                    section.IsSummer,
                    (int)section.YearLevel,
                    section.IsDeleted,
                    section.Course?.CourseId ?? (object)DBNull.Value
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@SectionsTVP", table.AsTableValuedParameter("dbo.SectionsTableType"));
            parameters.Add("@UserId", userId);

            int affectedRows = await _dbConnection.ExecuteAsync("InsertOrUpdateSections", parameters, commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }
    }
}