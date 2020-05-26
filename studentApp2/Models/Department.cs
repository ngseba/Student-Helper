using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public List<CourseDepartment> CourseDepartments { get; set; }
    }
}