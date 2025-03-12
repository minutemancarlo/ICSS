using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class FacultyModel : BaseModel
    {
        public int? FacultyId { get; set; }
        public string FacultyName { get; set; }
        public string AcademicRank { get; set; }        
        public decimal? TotalLoadUnits { get; set; }
        public string? BachelorsDegree { get; set; }
        public string? MastersDegree { get; set; }
        public string? DoctorateDegree { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal RemainingUnits { get; set; } = 0.00M;
    }

    public class FacultyWorkload
    {
        public FacultyModel? Faculty { get; set; }
        public Course? Course { get; set; }
        public Sections? Sections { get; set; }
        public ScheduleTimeSlot? ScheduleTimeSlot { get; set; }
        public Subjects? Subjects { get; set; }
        public Rooms? Rooms { get; set; }
    }

}

