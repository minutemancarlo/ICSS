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
        public int? DepartmentId { get; set; }
        public decimal? TotalLoadUnits { get; set; }
        public string? BachelorsDegree { get; set; }
        public string? MastersDegree { get; set; }
        public string? DoctorateDegree { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

