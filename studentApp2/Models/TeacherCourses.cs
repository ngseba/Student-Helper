using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class TeacherCourses
    {
        [Key]
        public int TeacherCoursesID { get; set; }
        [Index("UNIQUE_Teacher_Course", 1, IsUnique = true)]
        public int CourseID { get; set; }
        [Index("UNIQUE_Teacher_Course", 2, IsUnique = true)]
        public int TeacherID { get; set; }
        public virtual Course Course { get; set; }

        public virtual Teacher Teacher { get; set; }

        public List<TeacherCoursesGroup> TeacherCoursesGroups { get; set; }
        public override string ToString()
        {
            return  this.TeacherCoursesID.ToString() + " "; 
        }
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