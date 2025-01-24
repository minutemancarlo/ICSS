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

        public async Task<IEnumerable<Tasks>> GetOnQueueTasksAsync()
        {
            var query = "SELECT * FROM Tasks WHERE status = @Status";
            var parameters = new DynamicParameters();
            parameters.Add("Status", TaskStatus.On_Queue);

            var tasks = await _dbConnection.QueryAsync<Tasks>(query, parameters);
            return tasks;
        }


        public async Task<bool> VerifyStudentExistenceAsync(string? IdNumber)
        {
            var query = "SELECT COUNT(*) FROM Students WHERE IdNumber = @IdNumber";
            var parameters = new DynamicParameters();
            parameters.Add("@IdNumber", IdNumber);

            var exist = await _dbConnection.QuerySingleAsync<int>(query, parameters);
            return exist > 0;
        }


        public async Task<bool> UpdateTaskStatusAsync(int? taskId, TaskStatus status)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var manilaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var query = "UPDATE Tasks SET Status = @Status, UpdatedOn = @UpdatedOn WHERE TaskId = @TaskId";

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId);
            parameters.Add("@Status", status);
            parameters.Add("@UpdatedOn", manilaTime);
            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return result > 0;
        }

    }

}
