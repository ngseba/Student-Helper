using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class CourseDepartment
    {
        [Key]
        public int ID { get; set; }
        [Index("UNIQUE_Course_Department", 1, IsUnique = true)]
        public int CourseID { get; set; }

        [Index("UNIQUE_Course_Department", 2, IsUnique = true)]
        public int DepartmentID { get; set; }

        public virtual Course Course { get; set; }

        public virtual Department Department { get; set; }
    }
}