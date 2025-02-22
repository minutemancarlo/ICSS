using System.Data;
using System.Data.Common;

namespace ICSS.Server.Repository
{

    public class ScheduleRepository
    {
        private readonly IDbConnection _dbConnection;

        public ScheduleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

    }
}
