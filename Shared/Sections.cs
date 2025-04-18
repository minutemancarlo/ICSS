﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSS.Shared
{
    public class Sections : BaseModel
    {
        public int? SectionId { get; set; }
        public string? SchoolYear { get; set; }
        public string? SectionName { get; set; }
        public bool IsSummer { get; set; } = false;
        public YearLevel? YearLevel { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? StudentCount { get; set; }

        public int? ClassSize { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }
    }


    public class SectionMember
    {
        public int? SectionId { get; set; }
        public int? StudentId { get; set; }
    }


    public class SectionResponse
    {
        public Sections? Sections { get; set; }
        public List<StudentModel>? Students { get; set; }
    }

    public enum YearLevel
    {
        First_Year = 1,
        Second_Year = 2,
        Third_Year = 3,
        Fourth_Year = 4

    }
}
