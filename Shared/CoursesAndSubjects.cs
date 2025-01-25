using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class Course:BaseModel
    {
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? CourseAcronym { get; set; }
        public bool IsDeleted { get; set; } = false;

    }

    public class Subjects : BaseModel
    {
        public int? SubjectId { get; set; }
        public string? SubjectCode { get; set; }
        public int? CourseId { get; set; }
        public string? SubjectName { get; set; }
        public int Units { get; set; } = 0;        

    }
}
