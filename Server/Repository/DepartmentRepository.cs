using Dapper;
using ICSS.Client.Pages.Admin;
using ICSS.Shared;
using System.Data;

namespace ICSS.Server.Repository
{
    public class DepartmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public DepartmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Departments>> GetDepartmentsAsync()
        {
            return await _dbConnection.QueryAsync<Departments>("GetDepartments", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertUpdateDepartmentAsync(Departments department)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DepartmentId", department.DepartmentId, DbType.Int32);
            parameters.Add("@DepartmentCode", department.DepartmentCode, DbType.String);
            parameters.Add("@DepartmentName", department.DepartmentName, DbType.String);
            parameters.Add("@IsDeleted", department.IsDeleted, DbType.Boolean);
            parameters.Add("@User", department.UpdatedBy ?? department.CreatedBy, DbType.String);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertUpdateDepartment", parameters, commandType: CommandType.StoredProcedure);
        }


    }
}
