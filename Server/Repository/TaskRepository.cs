using ICSS.Shared;
using System.Data.SqlClient;
using System.Data;
using TaskStatus = ICSS.Shared.TaskStatus;
using Dapper;

namespace ICSS.Server.Repository
{
    public class TasksRepository
    {
        private readonly IDbConnection _dbConnection;

        public TasksRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> InsertTaskAsync(Tasks task)
        {
            
            var parameters = new DynamicParameters();
            parameters.Add("@LogPath", task.LogPath, DbType.String);
            parameters.Add("@FileName", task.FileName, DbType.String);
            parameters.Add("@Status", task.Status, DbType.Int32);
            parameters.Add("@TaskType", task.TaskType, DbType.Int32);
            parameters.Add("@CreatedBy", task.CreatedBy, DbType.String);

            var result = await _dbConnection.ExecuteScalarAsync<int>("InsertTask", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<Tasks>> GetTasksAsync()
        {
            
            var query = "SELECT TaskId,LogPath,Status,FileName,TaskType,(Select Top 1 Name from Users where SystemId = CreatedBy) as CreatedBy,CreatedOn,UpdatedOn FROM Tasks";

            var tasks = await _dbConnection.QueryAsync<Tasks>(query);
            return tasks;
        }
    }

}
