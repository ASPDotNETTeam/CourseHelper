using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelper.Controllers
{
    [Area("Faculty")]
    [Authorize(Roles ="Admin, Faculty")]
    public class CourseController : Controller
    {
        private ICourseDatabase courseDB;
        private IStudentDatabase studentDB;
        private ICourseStudentDatabase csDB;

        public CourseController(ICourseDatabase courseDB, IStudentDatabase studentDB, ICourseStudentDatabase csDB)
        {
            this.courseDB = courseDB;
            this.studentDB = studentDB;
            this.csDB = csDB;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Display()
        {
            return View(courseDB.Courses);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View("Insert", new Course());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"Course {course.Name} successfully created!";
                return RedirectToAction(nameof(Display));
            }
            else
            {
                return View("Insert", course);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to edit it!";
                return RedirectToAction(nameof(Display));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(Display));
            }
            return View("Data", course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                courseDB.SaveCourse(course);
                TempData["SuccessMessage"] = $"Course {course.Name} successfully updated.";
                return RedirectToAction(nameof(Display));
            }
            else
            {
                TempData["errorMessage"] = "Wrong input, please check!";
                return View("Data", course);
            }
        }

        
        public IActionResult Manage(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to add student!";
                return RedirectToAction(nameof(Display));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(Display));
            }
            EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
            return View("User", enrollInfo);
        }

        [HttpPost]
        
        public IActionResult Manage(EnrollmentList list)
        {
            Course course = courseDB.Courses.FirstOrDefault(c => c.CourseId == list.CourseID);
            if (course != null)
            {
                if (ModelState.IsValid)
                {
                    if (list.EnrollQueue == null)
                    {
                        TempData["errorMessage"] = "Please select a valid student.";
                        EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
                        return View("User", enrollInfo);
                    }
                    foreach (long i in list.EnrollQueue)
                    {
                        Student student = studentDB.Students.FirstOrDefault(s => s.StudentNumber == i);
                        CourseStudent csdb = course.AddStudent(student);
                        studentDB.SaveStudent(student);
                        csDB.AddCourseStudents(csdb);
                    }
                    courseDB.SaveCourse(course);
                    TempData["successMessage"] = $"Student(s) successfully added to the course {course.Name}";
                    return RedirectToAction(nameof(Display));
                }
                else
                {
                    EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
                    return View("User", enrollInfo);
                }
            }
            else
            {
                TempData["errorMessage"] = "Please select a valid course!";
                return RedirectToAction(nameof(Display));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int courseID)
        {
            Course deletedCourse = courseDB.DeleteCourse(courseID);
            if (deletedCourse != null)
            {
                TempData["successMessage"] = $"Course {deletedCourse.Name} successfully deleted";
            }
            
            return RedirectToAction(nameof(Display));
        }

        [NonAction]
        private EnrollmentInfo CreateEnrollInfo(Course course)
        {
            ICollection<Student> unenrolledStudents = new List<Student>();
            ICollection<Student> enrolledStudents = new List<Student>();
            foreach (Student dbStudent in studentDB.Students)
            {
                //var list = course.Students.Contains(dbStudent) ? unenrolledStudents : enrolledStudents;
                //list.Add(dbStudent);
                if (course.Students.Contains(dbStudent))
                {
                    enrolledStudents.Add(dbStudent);
                }
                else
                {
                    unenrolledStudents.Add(dbStudent);
                }
            }
            return new EnrollmentInfo() { Course = course, EnrolledStudents = enrolledStudents, UnenrolledStudents = unenrolledStudents };
        }
    }
}