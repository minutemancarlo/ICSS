using System.Data;
using System.Data.SqlClient;
using Dapper;
using ICSS.Shared;

namespace ICSS.Server.Repository
{
    public class StudentRepository
    {
        private readonly IDbConnection _dbConnection;

        public StudentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> InsertStudentAsync(StudentModel student)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", student.Name);
            parameters.Add("@IdNumber", student.IdNumber);
            parameters.Add("@IsActive", student.IsActive);
            parameters.Add("@SystemId", student.SystemId);
            parameters.Add("@Email", student.Email);
            parameters.Add("@CreatedBy", student.CreatedBy);

            return await _dbConnection.ExecuteScalarAsync<int>("InsertStudent", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStudentAsync(StudentModel student)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", student.Id);
            parameters.Add("@Name", student.Name);
            parameters.Add("@IdNumber", student.IdNumber);
            parameters.Add("@IsActive", student.IsActive);
            parameters.Add("@SystemId", student.SystemId);
            parameters.Add("@Email", student.Email);
            parameters.Add("@UpdatedBy", student.UpdatedBy);

            await _dbConnection.ExecuteAsync("UpdateStudent", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StudentModel>> GetStudentsAsync()
        {
            return await _dbConnection.QueryAsync<StudentModel>("GetStudents", commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> VerifyStudentAsync(string email, string idNumber, int id)
        { 
            var query = string.Empty;
            if (id == 0)
            {
                query = @"
                SELECT COUNT(1) 
                    FROM Students 
                    WHERE Email = @Email 
                        OR IdNumber = @IdNumber";
            }
            else
            {
                query = @"
                SELECT COUNT(1) 
                    FROM Students 
                    WHERE (Email = @Email 
                        OR IdNumber = @IdNumber )
                        AND Id != @Id";
            }
            

            var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { Email = email, IdNumber = idNumber, Id = id });
            return result > 0;
        }

    }
}
