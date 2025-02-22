using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class ScheduleRequest : BaseModel
    {
        public int? TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public TaskType TaskType { get; set; }        
        public Course? Course { get; set; }
        
        

    }
}
