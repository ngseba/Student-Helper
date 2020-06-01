using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class TeacherCoursesGroup
    {
        [Key]
        public int TeacherCoursesGroupID { get; set; }
        public int TeacherCoursesID { get; set; }
        public TeacherCourses TeacherCourses { get; set; }
        public int GroupID { get; set; }
        public Group Group { get; set; }
    }

    public class TCGViewModel
    {
        public int TeacherCourseGroupID { get; set; }
        public TeacherNameViewModel tName {get;set;}
        public string groupName { get; set; }
    }
}