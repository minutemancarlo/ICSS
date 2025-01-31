using Dapper;
using ICSS.Shared;
using System.Data;

namespace ICSS.Server.Repository
{
    public class FacultyRepository
    {
        private readonly IDbConnection _dbConnection;

        public FacultyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<FacultyModel>> GetFacultyAsync()
        {
            return await _dbConnection.QueryAsync<FacultyModel>("GetFaculty", commandType: CommandType.StoredProcedure);
        }


        public async Task<int> InsertUpdateFacultyAsync(FacultyModel faculty)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", faculty.FacultyId, DbType.Int32);
            parameters.Add("@FacultyName", faculty.FacultyName, DbType.String);
            parameters.Add("@AcademicRank", faculty.AcademicRank, DbType.String);
            parameters.Add("@DepartmentId", faculty.DepartmentId, DbType.Int32);
            parameters.Add("@TotalLoadUnits", faculty.TotalLoadUnits, DbType.Decimal);
            parameters.Add("@BachelorsDegree", faculty.BachelorsDegree, DbType.String);
            parameters.Add("@MastersDegree", faculty.MastersDegree, DbType.String);
            parameters.Add("@DoctorateDegree", faculty.DoctorateDegree, DbType.String);
            parameters.Add("@IsDeleted", faculty.IsDeleted, DbType.Boolean);
            parameters.Add("@User", faculty.UpdatedBy ?? faculty.CreatedBy, DbType.String);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertUpdateFaculty", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
