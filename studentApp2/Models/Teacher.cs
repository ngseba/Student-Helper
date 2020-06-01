using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        public List<TeacherCourses> TeacherCourses { get; set; }
        [Required]
        public string UserId { get; set; }
    }

    public class UserTeacherViewModel
    {
        public Teacher Teacher { get; set; }
        public string Username { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
    }
}