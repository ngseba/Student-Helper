using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class TeacherCoursesGroup
    {
        [Key]
        public int TeacherCoursesGroupID { get; set; }
        public int TeacherCoursesID { get; set; }
        [Index("UNIQUE_Teacher_Course_Group", 1,IsUnique = true)]

        public int GroupID { get; set; }
        [Index("UNIQUE_Teacher_Course_Group", 2, IsUnique = true)]

        public virtual TeacherCourses TeacherCourses { get; set; }

        public virtual Group Group { get; set; }
    }

    public class TCGViewModel
    {
        public int TeacherCourseGroupID { get; set; }
        public TeacherNameViewModel tName {get;set;}
        public string groupName { get; set; }
    }
}