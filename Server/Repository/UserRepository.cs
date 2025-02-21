using Dapper;
using ICSS.Shared;
using System.Data;

namespace ICSS.Server.Repository
{
    public class UserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> CheckAndInsertSystemIdAsync(string systemId, string name, string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SystemId", systemId, DbType.String);
            parameters.Add("@Name", name, DbType.String);
            parameters.Add("@Email", email, DbType.String);

            await _dbConnection.ExecuteAsync("CheckAndInsertSystemId", parameters, commandType: CommandType.StoredProcedure);

            return true; 
        }

        public async Task<bool> UpdateAdminDepartmentAsync(int? departmentId,string? userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.String);
            parameters.Add("@DepartmentId", departmentId, DbType.Int32);            

            await _dbConnection.ExecuteAsync("UpdateAdminDepartment", parameters, commandType: CommandType.StoredProcedure);

            return true;
        }

        public async Task<Departments?> CheckUserDepartment(string? userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.String);

            return await _dbConnection.QueryFirstOrDefaultAsync<Departments>(
                "GetAdminDepartment",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
 


    }
}
 