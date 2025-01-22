using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class StudentModel :BaseModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? IdNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public string? SystemId { get; set; }
        public string? Email { get; set; }
        
    }
}
