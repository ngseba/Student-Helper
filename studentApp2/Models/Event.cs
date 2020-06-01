using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public virtual TeacherCourses TeacherCourses { get; set; }
        public int TeacherCoursesID { get; set; }

    }

    public class EventTeacherViewModel
    {
        public string EventTitle { get; set; }
        public string EventType { get; set; }
        public int EventCourse { get; set; }
        public int EventGroup { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
    }

    public class EventCalendarViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public string description { get; set; }
        public string courseName { get; set; }
        public string teacherName { get; set; }
        public string teacherUsername { get; set; }

    }

    public enum EventType
    {
        Assignment,
        Project,
        Exam
    }
}