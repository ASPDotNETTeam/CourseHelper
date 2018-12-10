using System;
using System.Linq;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelper.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private ICourseDatabase courseDB;
        private IStudentDatabase studentDB;
        private ICourseStudentDatabase csDB;
        private IReviewDatabase reviewDB;

        public StudentController(ICourseDatabase courseDB, IStudentDatabase studentDB, ICourseStudentDatabase csDB, IReviewDatabase reviewDB)
        {
            this.courseDB = courseDB;
            this.studentDB = studentDB;
            this.csDB = csDB;
            this.reviewDB = reviewDB;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DisplayCourses()
        {
            return View("Display",courseDB.Courses);
        }

        public IActionResult Enrol(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to add student!";
                return RedirectToAction(nameof(DisplayCourses));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(DisplayCourses));
            }
            // get student info from the identity info
            string userName = User.Identity.Name.ToString();
            Student student = studentDB.Students.FirstOrDefault(s => s.FirstName == userName);
            if(student == null)
            {
                TempData["errorMessage"] = "no student data found";
                return RedirectToAction(nameof(DisplayCourses));
            }
            if (course.Students.Contains(student))
            {
                TempData["errorMessage"] = "You already enrolled this course";
                return RedirectToAction(nameof(MyCourses));
            }
            if (!course.Students.Contains(student))
            {
                CourseStudent csdb = course.AddStudent(student);
                studentDB.SaveStudent(student);
                csDB.AddCourseStudents(csdb);
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"Enrollment completed! You have added to course {courseCode}";
                return RedirectToAction(nameof(MyCourses));
            }
            return RedirectToAction(nameof(DisplayCourses));
        }

        public IActionResult MyCourses()
        {
            string userName = User.Identity.Name.ToString();
            Student student = studentDB.Students.FirstOrDefault(s => s.FirstName == userName);
            if (student == null)
            {
                TempData["errorMessage"] = "no student data found";
                return RedirectToAction(nameof(DisplayCourses));
            }
            return View(courseDB.Courses.Where(c => c.Students.Contains(student)));
        }

        public IActionResult Drop(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to add student!";
                return RedirectToAction(nameof(DisplayCourses));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(DisplayCourses));
            }
            string userName = User.Identity.Name.ToString();
            Student student = studentDB.Students.FirstOrDefault(s => s.FirstName == userName);
            if (student == null)
            {
                TempData["errorMessage"] = "no student data found";
                return RedirectToAction(nameof(DisplayCourses));
            }
            // drop student from the course
            // TempData["successMessage"] = "Course successfully dropped";
            return RedirectToAction(nameof(MyCourses));
        }

        public IActionResult ReviewForm(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to add student!";
                return RedirectToAction(nameof(DisplayCourses));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(DisplayCourses));
            }
            Review review = new Review() { CourseCode = courseCode, CourseName = course.Name};
            return View(review);
        }

        public IActionResult ReviewList()
        {
            return View(reviewDB.Reviews);
        }

        [HttpPost]
        public IActionResult ReviewList(Review review)
        {
            if (review.CreatorName == "true")
            {
                review.CreatorName = User.Identity.Name.ToString();
            }
            else if (review.CreatorName == "false")
            {
                review.CreatorName = "Anonymous";
            }
            review.CreateTime = DateTime.Now;
            reviewDB.SavaReview(review);
            TempData["successMessage"] = "Review successfully submitted";
            return View(reviewDB.Reviews);
        }
    }
}