using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class CourseDepartment
    {
        [Key]
        public int ID { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
    }
}