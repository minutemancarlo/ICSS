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
        public string? CourseCode { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Departments? Departments { get; set; }
    }

    public class Subjects : BaseModel
    {
        public int? SubjectId { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public decimal LectureHour { get; set; } = 0.0M;
        public decimal LabHour { get; set; } = 0.0M;
        public int MaxStudent { get; set; } = 0;
        public decimal Units { get; set; } = 0.0M;        
        public int? CourseId { get; set; }
        public bool IsActive { get; set; } = true;
        public Departments? Departments { get; set; }
        public YearLevel? YearLevel { get; set; }
        public Semester? Semester { get; set; }
        public bool IsSaturdayClass { get; set; } = false;

        //For Process
        public int? DepartmentId { get; set; }

    }


    public enum Semester
    {
        First_Semester = 1,
        Second_Semester = 2
    }
}
