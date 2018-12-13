using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.Database;
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

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult List()
        {
            return View(courseDB.Courses);
        }
        
        public IActionResult Create()
        {
            return View("Create", new Course());
        }

        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"Course {course.Name} successfully created!";
                return RedirectToAction(nameof(List));
            }
            else
            {
                return View("Create", course);
            }
        }
        
        public IActionResult Edit(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "Please select a course to edit.";
                return RedirectToAction(nameof(List));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course to edit.";
                return RedirectToAction(nameof(List));
            }
            return View("Edit", course);
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                courseDB.SaveCourse(course);
                TempData["SuccessMessage"] = $"Course {course.Name} successfully updated.";
                return RedirectToAction(nameof(List));
            }
            else
            {
                TempData["errorMessage"] = "Something went wrong, please try again.";
                return View("Edit", course);
            }
        }
        
        public IActionResult Manage(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "Please select a course to manage enrollment.";
                return RedirectToAction(nameof(List));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course to manage enrollment.";
                return RedirectToAction(nameof(List));
            }
            EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
            return View("Enrollment", enrollInfo);
        }

        [HttpPost]
        public IActionResult Manage(EnrollmentList list)
        {
            Course course = courseDB.Courses.FirstOrDefault(c => c.CourseId == list.CourseID);
            if (course != null)
            {
                if (ModelState.IsValid)
                {
                    foreach (long i in list.EnrollQueue)
                    {
                        Student student = studentDB.Students.FirstOrDefault(s => s.StudentNumber == i);
                        if (!course.Students.Contains(student))
                        {
                            CourseStudent csdb = course.AddStudent(student);
                            studentDB.SaveStudent(student);
                            csDB.AddCourseStudents(csdb);
                        }
                    }
                    foreach (long i in list.UnEnrollQueue)
                    {
                        Student student = studentDB.Students.FirstOrDefault(s => s.StudentNumber == i);
                        if (course.Students.Contains(student))
                        {
                            CourseStudent csdb = course.DeleteStudent(student);
                            studentDB.SaveStudent(student);
                            csDB.DeleteCourseStudents(csdb);
                        }
                    }
                    courseDB.SaveCourse(course);
                    TempData["successMessage"] = $"Enrollment changes successfully saved.";
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    EnrollmentInfo enrollInfo = CreateEnrollInfo(course);
                    return View("Enrollment", enrollInfo);
                }
            }
            else
            {
                TempData["errorMessage"] = "Please select a valid course to manage enrollment.";
                return RedirectToAction(nameof(List));
            }
        }

        [HttpPost]
        public IActionResult Delete(int courseID)
        {
            Course deletedCourse = courseDB.DeleteCourse(courseID);
            if (deletedCourse != null)
            {
                TempData["successMessage"] = $"Course {deletedCourse.Name} successfully deleted.";
            }
            
            return RedirectToAction(nameof(List));
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