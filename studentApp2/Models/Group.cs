using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        [Required]
        public string GroupName { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]
        public int YearOfStudy { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual Department Department { get; set; }
        public List<TeacherCoursesGroup> TeacherCoursesGroup { get; set; }
    }
}