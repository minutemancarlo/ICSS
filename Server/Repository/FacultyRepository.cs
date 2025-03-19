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

        public async Task<int> UpdateFacultyStatusAsync(int facultyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", facultyId);
            return await _dbConnection.ExecuteScalarAsync<int>("UpdateFacultyStatus", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetFacultyIdAsync(string? id)
        {
            var query = "SELECT TOP 1 B.FacultyId FROM Users A INNER JOIN Faculty B ON A.Email = B.Email WHERE A.SystemId = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<int>(query, parameters);

            return result;
        }


        public async Task<IEnumerable<DepartmentMember>> GetFacultyAsync()
        {
            var result = await _dbConnection.QueryAsync<FacultyModel, Departments, DepartmentMember>(
                "GetFaculty",
                (faculty, department) =>
                {
                    return new DepartmentMember
                    {
                        FacultyModel = faculty,
                        Departments = department
                    };
                },
                splitOn: "DepartmentId",
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<DepartmentMember>> GetFacultyAssignedAsync(int departmentId)
        {
            var parameters = new { DepartmentId = departmentId };

            var result = await _dbConnection.QueryAsync<FacultyModel, Departments, DepartmentMember>(
                "GetFacultyAssigned",
                (faculty, department) =>
                {
                    return new DepartmentMember
                    {
                        FacultyModel = faculty,
                        Departments = department
                    };
                },
                param: parameters,
                splitOn: "DepartmentId",
                commandType: CommandType.StoredProcedure
            );

            return result;
        }



        public async Task<IEnumerable<DepartmentMember>> GetFacultyNotMemberAsync()
        {
            string query = @"
        SELECT 
            F.*, DM.DepartmentId            
        FROM Faculty F
        LEFT JOIN DepartmentMember DM ON F.FacultyId = DM.FacultyId
        WHERE DM.DepartmentId IS NULL;";

            var result = await _dbConnection.QueryAsync<FacultyModel, DepartmentMember, DepartmentMember>(
                query,
                (faculty, departmentMember) =>
                {
                    departmentMember.FacultyModel = faculty;
                    return departmentMember;
                },
                splitOn: "DepartmentId",
                commandType: CommandType.Text
            );

            return result;
        }


        public async Task<int> InsertUpdateFacultyAsync(FacultyModel faculty)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FacultyId", faculty.FacultyId, DbType.Int32);
            parameters.Add("@FacultyName", faculty.FacultyName, DbType.String);
            parameters.Add("@Email", faculty.Email, DbType.String);
            parameters.Add("@AcademicRank", faculty.AcademicRank, DbType.String);            
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
