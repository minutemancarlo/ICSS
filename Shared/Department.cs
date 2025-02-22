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


    public class Rooms : BaseModel
    {
        public int? RoomId { get; set; }
        public string? RoomCode { get; set; }
        public string? RoomName { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public bool IsLab { get; set; } = false;
        public Departments? Departments { get; set; }
    }
}
