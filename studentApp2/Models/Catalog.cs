using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace studentApp2.Models
{
    public class Catalog
    {
        [Key]
        public int GradeID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int Grade { get; set; }
        public DateTime GradeDate { get; set; }
    }

    public class CatalogTeacherViewModel
    {

        public List<UserStudentViewModel> Students { get; set; }
        public int GradeID { get; set; }
        public int Grade { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }
    }

    public class CatalogStudentViewModel
    {
        public List<GradeStudentViewModel> grades { get; set; }
    }

    public class GradeStudentViewModel
    {
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public DateTime GradeDate { get; set; }
    }
}