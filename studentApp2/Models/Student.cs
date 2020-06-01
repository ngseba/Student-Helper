using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Student
    {

        public int StudentId { get; set; }
        public int? GroupID { get; set; }
        public virtual Group Group { get; set; }

        public virtual ApplicationUser User { get; set; }
        [Required]
        public string UserId { get; set; }
    }

    public class UserStudentViewModel
    {
        public int StudentId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string GroupName { get; set; }
        public int? YearOfStudy { get; set; }
        public int? Grade { get; set; }

        public int GradeID { get; set; }
        public string GradeDate { get; set; }
    }
}