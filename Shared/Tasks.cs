using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class Tasks: BaseModel
    {
        public int? TaskId { get; set; }               
        public string? LogPath { get; set; }
        public string? FileName { get; set; }
        public TaskStatus Status { get; set; }
        public TaskType TaskType { get; set; }                
    }

    public enum TaskStatus { On_Queue,Processing,Success, Failed, Cancelled }
    public enum TaskType {Student,Faculty,Schedule,Per_Department_Schedule }

}
