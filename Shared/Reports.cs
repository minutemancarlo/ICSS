using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class ReportSchedule
    {
        public string? SchoolYear { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectDescription { get; set; }
        public decimal? Units { get; set; }
        public decimal? LoadUnits { get; set; }
        public string? Schedule { get; set; }
        public string? Days { get; set; }
        public string? Room { get; set; }
        public string? Instructor { get; set; }
        public string? College { get; set; }
    }

}
