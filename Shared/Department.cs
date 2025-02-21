using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class Departments: BaseModel
    {
        public int? DepartmentId { get; set; } 
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsDeleted { get; set; } = false;

    }

    public class  DepartmentMember: BaseModel
    {
        public int? Id { get; set; }        
        public FacultyModel? FacultyModel { get;set; }
        public Departments? Departments { get; set; }
    }

}
