using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        public string CourseName { get; set; }
        public int CourseYear { get; set; }
        public List<CourseDepartment> CourseDepartments { get; set; }
        public List<TeacherCourses> TeacherCourses { get; set; }
    }
}