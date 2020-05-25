using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class TeacherCourses
    {
        [Key]
        public int TeacherCoursesID { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int TeacherID { get; set; }
        public Teacher Teacher { get; set; }

        public List<TeacherCoursesGroup> TeacherCoursesGroups { get; set; }
    }

    public class TeacherNameViewModel
    {
        public int TCId { get; set; }
        public int TeacherId { get; set; }
        public string UserId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }

    public class QueryResults
    {
        public int Value { get; set; } 
        public string Text { get; set; }
    }
}