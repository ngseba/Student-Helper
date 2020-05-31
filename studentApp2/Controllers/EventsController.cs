using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using studentApp2.Models;

namespace studentApp2.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            if (User.IsInRole("Student"))
            {
                var userId = User.Identity.GetUserId();
                var currentStudent = db.Students.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
                var groupEvents = db.GroupEvents.Where(groupEvent => groupEvent.GroupID == currentStudent.GroupID)
                    .Select(groupEvent => new EventCalendarViewModel
                    {
                        title = groupEvent.Event.EventTitle,
                        type = groupEvent.Event.EventType,
                        description = groupEvent.Event.EventDescription,
                        start = groupEvent.Event.EventDate,
                        courseName = groupEvent.Event.TeacherCourses.Course.CourseName,
                        teacherName = groupEvent.Event.TeacherCourses.Teacher.User.Firstname
                        + " " + groupEvent.Event.TeacherCourses.Teacher.User.Lastname,
                        teacherUsername = groupEvent.Event.TeacherCourses.Teacher.User.UserName
                    }).ToList();
                ViewBag.groupEvents = groupEvents;

            }
            else
            if (User.IsInRole("Teacher"))
            {
                ViewBag.TeacherCourses = new SelectList(getCourseList(), "CourseID", "CourseName");

            }
            return View();

        }



        public List<Course> getCourseList()
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var courseList = db.TeacherCourses
                .Where(teacherCourses => teacherCourses.TeacherID == currentTeacher.TeacherId)
                .Select(teacherCourses => teacherCourses.Course)
                .ToList();

            ViewBag.CourseList = new SelectList(courseList);

            return courseList;


        }

        [Authorize(Roles = "Teacher")]

        public String getGroupList(int id)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var teacherCourseGroupList = db.TeacherCoursesGroups
                .Where(teacherCourseGroup => teacherCourseGroup.TeacherCourses.TeacherID == currentTeacher.TeacherId
                && teacherCourseGroup.TeacherCourses.CourseID == id)
                .Select(teacherCourseGroup => new {
                    GroupID = teacherCourseGroup.Group.GroupID,
                    GroupName = teacherCourseGroup.Group.GroupName
                }).ToList();

            string output = JsonConvert.SerializeObject(teacherCourseGroupList);
            return output;

        }


        public String getEventList(int id)
        {
            var groupEvents = db.GroupEvents.Where(groupEvent => groupEvent.GroupID == id)
                   .Select(groupEvent => new EventCalendarViewModel
                   {
                       id = groupEvent.Event.EventID,
                       title = groupEvent.Event.EventTitle,
                       type = groupEvent.Event.EventType,
                       description = groupEvent.Event.EventDescription,
                       start = groupEvent.Event.EventDate,
                       courseName = groupEvent.Event.TeacherCourses.Course.CourseName,
                       teacherName = groupEvent.Event.TeacherCourses.Teacher.User.Firstname
                       + " " + groupEvent.Event.TeacherCourses.Teacher.User.Lastname,
                       teacherUsername = groupEvent.Event.TeacherCourses.Teacher.User.UserName

                   }).ToList();
            return JsonConvert.SerializeObject(groupEvents);
        }


        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public String Create([Bind(Include = "EventCourse,EventDate,EventDescription,EventGroup,EventTitle,EventType")] EventTeacherViewModel eventTeacherViewModel)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var TeacherCoursesID = db.TeacherCourses.Where(teacherCourses =>
                teacherCourses.TeacherID == currentTeacher.TeacherId &&
                teacherCourses.CourseID == eventTeacherViewModel.EventCourse
            ).FirstOrDefault().TeacherCoursesID;
            if (ModelState.IsValid)
            {
                var newEvent = new Event {
                    EventTitle = eventTeacherViewModel.EventTitle,
                    EventType = eventTeacherViewModel.EventType,
                    EventDate = eventTeacherViewModel.EventDate,
                    EventDescription = eventTeacherViewModel.EventDescription,
                    TeacherCoursesID = TeacherCoursesID
                };
               
                db.Events.Add(newEvent);
                db.SaveChanges();
                db.GroupEvents.Add(new GroupEvent
                {
                    EventID = newEvent.EventID,
                    GroupID = eventTeacherViewModel.EventGroup
                });
                db.SaveChanges();
                var addedEvent = db.GroupEvents.Where(groupEvent => groupEvent.EventID == newEvent.EventID)
                   .Select(groupEvent => new EventCalendarViewModel
                   {
                       id = groupEvent.Event.EventID,
                       title = groupEvent.Event.EventTitle,
                       type = groupEvent.Event.EventType,
                       description = groupEvent.Event.EventDescription,
                       start = groupEvent.Event.EventDate,
                       courseName = groupEvent.Event.TeacherCourses.Course.CourseName,
                       teacherName = groupEvent.Event.TeacherCourses.Teacher.User.Firstname
                       + " " + groupEvent.Event.TeacherCourses.Teacher.User.Lastname,
                       teacherUsername = groupEvent.Event.TeacherCourses.Teacher.User.UserName

                   }).FirstOrDefault();
                return JsonConvert.SerializeObject(addedEvent);
            }
            return JsonConvert.SerializeObject(null);
        }



        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public String Edit([Bind(Include = "EventID,EventTitle,EventType,EventDate,EventDescription")] Event editEvent)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            if (ModelState.IsValid)
            {
                Event dbEvent = db.Events.Find(editEvent.EventID);
                var eventAuthor = dbEvent.TeacherCourses.Teacher;
                if(currentTeacher.TeacherId == eventAuthor.TeacherId)
                {
                    dbEvent.EventTitle = editEvent.EventTitle;
                    dbEvent.EventType = editEvent.EventType;
                    dbEvent.EventDate = editEvent.EventDate;
                    dbEvent.EventDescription = editEvent.EventDescription;
                    db.SaveChanges();
                    var editedEvent = db.GroupEvents.Where(groupEvent => groupEvent.EventID == dbEvent.EventID)
                       .Select(groupEvent => new EventCalendarViewModel
                       {
                           id = groupEvent.Event.EventID,
                           title = groupEvent.Event.EventTitle,
                           type = groupEvent.Event.EventType,
                           description = groupEvent.Event.EventDescription,
                           start = groupEvent.Event.EventDate,
                           courseName = groupEvent.Event.TeacherCourses.Course.CourseName,
                           teacherName = groupEvent.Event.TeacherCourses.Teacher.User.Firstname
                           + " " + groupEvent.Event.TeacherCourses.Teacher.User.Lastname,
                           teacherUsername = groupEvent.Event.TeacherCourses.Teacher.User.UserName

                       }).FirstOrDefault();

                    return JsonConvert.SerializeObject(editedEvent);
                }
                return JsonConvert.SerializeObject(null);
            }
            return JsonConvert.SerializeObject(null);

        }


        // POST: Events/Delete/5
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            Event dbEvent = db.Events.Find(id);
            var eventAuthor = dbEvent.TeacherCourses.Teacher;
            if (currentTeacher.TeacherId == eventAuthor.TeacherId)
            {
                db.Events.Remove(db.Events.Find(id));
                db.SaveChanges();
                var data = new
                {
                    id
                };
                return Json(data);
            }
            return Json(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
