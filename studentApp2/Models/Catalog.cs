using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace studentApp2.Models
{
    public class Catalog
    {
        [Key]
        public int GradeID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int Grade { get; set; }
        public DateTime? GradeDate { get; set; }
    }

    public class CatalogTeacherViewModel
    {

        public List<UserStudentViewModel> Students { get; set; }
        public int GradeID { get; set; }
        public int Grade { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; }

        public IEnumerable<SelectListItem> gradeList = new SelectList(Enumerable.Range(1, 10).
                   Select(grade => new SelectListItem { Text = grade.ToString(), Value = grade.ToString() }));
    }

    public class CatalogStudentViewModel
    {
        public List<GradeStudentViewModel> grades { get; set; }
    }

    public class GradeStudentViewModel
    {
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public DateTime? GradeDate { get; set; }
    }
}