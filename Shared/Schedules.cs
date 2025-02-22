using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class ScheduleRequest : BaseModel
    {
        public int? ScheduleId { get; set; }
        public Course? Course { get; set; }
        public YearLevel? YearLevel { get; set; }
        public Sections? Section { get; set; }
        public Semester? Semester { get; set; }
        public Departments? Departments { get; set; }
        public bool IsActive { get; set; } = true;
        public TaskStatus? TaskStatus { get; set; }
        public List<ScheduleTimeSlot> TimeSlots { get; set; } = new List<ScheduleTimeSlot>();
    }

    public class ScheduleTimeSlot
    {
        public int TimeSlotId { get; set; }
        public int SubjectId { get; set; }
        public int RoomId { get; set; }
        public int? FacultyId { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

}
