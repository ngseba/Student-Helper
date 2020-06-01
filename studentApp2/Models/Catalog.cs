using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace studentApp2.Models
{
    public class Catalog
    {
        [Key]
        public int GradeID { get; set; }
        [Index("UNIQUE_Student_Course", 1, IsUnique = true)]
        public int StudentID { get; set; }

        [Index("UNIQUE_Student_Course", 2, IsUnique = true)]
        public int CourseID { get; set; }

        public int Grade { get; set; }
        public DateTime? GradeDate { get; set; }
    }


    public class GradeStudentViewModel
    {
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public DateTime? GradeDate { get; set; }
    }
}