using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelper.Controllers
{
    [Area("Student")]
    public class StudentController : Controller
    {
        private ICourseDatabase courseDB;
        private IStudentDatabase studentDB;
        private ICourseStudentDatabase csDB;

        public StudentController(ICourseDatabase courseDB, IStudentDatabase studentDB, ICourseStudentDatabase csDB)
        {
            this.courseDB = courseDB;
            this.studentDB = studentDB;
            this.csDB = csDB;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DisplayCourses()
        {
            return View(courseDB.Courses);
        }

    //    public IActionResult Create()
    //    {
    //        return View("Insert", new Course());
    //    }

    //    [HttpPost]
    //    public IActionResult Create(Course course)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            courseDB.SaveCourse(course);
    //            return RedirectToAction(nameof(Display));
    //        }
    //        else
    //        {
    //            return View("Insert", course);
    //        }
    //    }

    //    public IActionResult Edit(string courseCode)
    //    {
    //        if (courseCode == null)
    //        {
    //            return RedirectToAction(nameof(Display));
    //        }
    //        Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
    //        if (course == null)
    //        {
    //            return RedirectToAction(nameof(Display));
    //        }
    //        return View("Data", course);
    //    }

    //    [HttpPost]
    //    public IActionResult Edit(Course course)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            courseDB.SaveCourse(course);
    //            return RedirectToAction(nameof(Display));
    //        }
    //        else
    //        {
    //            return View("Data", course);
    //        }
    //    }

    //    public IActionResult Manage(string courseCode)
    //    {
    //        if (courseCode == null)
    //        {
    //            return RedirectToAction(nameof(Display));
    //        }
    //        Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
    //        if (course == null)
    //        {
    //            return RedirectToAction(nameof(Display));
    //        }
    //        EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
    //        return View("User", enrollInfo);
    //    }

    //    [HttpPost]
    //    public IActionResult Manage(EnrollmentList list)
    //    {
    //        Course course = courseDB.Courses.FirstOrDefault(c => c.CourseId == list.CourseID);
    //        if (course != null)
    //        {
    //            if (ModelState.IsValid)
    //            {
    //                foreach (long i in list.EnrollQueue)
    //                {
    //                    Student student = studentDB.Students.FirstOrDefault(s => s.StudentNumber == i);
    //                    CourseStudent csdb = course.AddStudent(student);
    //                    studentDB.SaveStudent(student);
    //                    csDB.AddCourseStudents(csdb);
    //                }
    //                courseDB.SaveCourse(course);
    //                return RedirectToAction(nameof(Display));
    //            }
    //            else
    //            {
    //                EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
    //                return View("User", enrollInfo);
    //            }
    //        }
    //        else
    //        {
    //            return RedirectToAction(nameof(Display));
    //        }
    //    }

    //    [HttpPost]
    //    public IActionResult Delete(int courseID)
    //    {
    //        Course deletedCourse = courseDB.DeleteCourse(courseID);
    //        if (deletedCourse != null)
    //        {
    //            //TODO: Confirmation of deletion (ViewData?)
    //        }
    //        return RedirectToAction(nameof(Display));
    //    }

    //    [NonAction]
    //    private EnrollmentInfo CreateEnrollInfo(Course course)
    //    {
    //        ICollection<Student> unenrolledStudents = new List<Student>();
    //        ICollection<Student> enrolledStudents = new List<Student>();
    //        foreach (Student dbStudent in studentDB.Students)
    //        {
    //            if (course.Students.Contains(dbStudent))
    //            {
    //                enrolledStudents.Add(dbStudent);
    //            }
    //            else
    //            {
    //                unenrolledStudents.Add(dbStudent);
    //            }
    //        }
    //        return new EnrollmentInfo() { Course = course, EnrolledStudents = enrolledStudents, UnenrolledStudents = unenrolledStudents };
    //    }
    }
}