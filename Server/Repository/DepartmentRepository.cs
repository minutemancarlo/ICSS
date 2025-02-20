using Dapper;
using ICSS.Client.Pages.Admin;
using ICSS.Shared;
using System.Data;
using System.Text;

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

        public async Task SaveDepartmentMembersAsync(List<DepartmentMember> members, string action)
        {
            var query = new StringBuilder();
            var parameters = new DynamicParameters();

            for (int i = 0; i < members.Count; i++)
            {
                if (action.Equals("insert", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
                    INSERT INTO DepartmentMember (FacultyId, DepartmentId)
                    VALUES (@FacultyId{i}, @DepartmentId{i});");

                    parameters.Add($"FacultyId{i}", members[i].FacultyModel.FacultyId);
                    parameters.Add($"DepartmentId{i}", members[i].Departments.DepartmentId);
                }
                else if (action.Equals("delete", StringComparison.OrdinalIgnoreCase))
                {
                    query.AppendLine($@"
                    DELETE FROM DepartmentMember 
                    WHERE FacultyId = @FacultyId{i} AND DepartmentId = @DepartmentId{i};");

                    parameters.Add($"FacultyId{i}", members[i].FacultyModel.FacultyId);
                    parameters.Add($"DepartmentId{i}", members[i].Departments.DepartmentId);
                }
            }

            await _dbConnection.ExecuteAsync(query.ToString(), parameters);
        }

        public async Task<IEnumerable<FacultyModel>> GetDepartmentMembers(int? departmentId)
        {
            var query = "SELECT FacultyId, DepartmentId FROM DepartmentMember WHERE DepartmentId = @DepartmentId";
                        
            return await _dbConnection.QueryAsync<FacultyModel>(query, new { DepartmentId = departmentId });
        }



    }
}
