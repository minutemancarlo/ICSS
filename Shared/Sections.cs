using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class Sections: BaseModel
    {
        public int? SectionId { get; set; }
        public string? SchoolYear { get; set; }
        public string? SectionName { get; set; }
        public bool IsSummer { get; set; } = false;
        public YearLevel YearLevel { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? StudentCount { get; set; }
        public Course? Course { get; set; }
    }


    public enum YearLevel
    {        
        First_Year=1,
        Second_Year=2,
        Third_Year=3,
        Fourth_Year=4
        
    }
}
