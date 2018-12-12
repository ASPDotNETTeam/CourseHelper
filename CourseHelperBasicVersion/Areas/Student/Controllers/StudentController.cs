using System;
using System.Linq;
using System.Threading.Tasks;
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
        private UserManager<CourseHelperUser> userManager;

        public StudentController(ICourseDatabase courseDB, IStudentDatabase studentDB, ICourseStudentDatabase csDB, IReviewDatabase reviewDB, UserManager<CourseHelperUser> userManager)
        {
            this.courseDB = courseDB;
            this.studentDB = studentDB;
            this.csDB = csDB;
            this.reviewDB = reviewDB;
            this.userManager = userManager;
        }

        [NonAction]
        private async Task<Student> getLoggedInStudent()
        {
            CourseHelperUser loggedInUser = await userManager.GetUserAsync(HttpContext.User);
            return studentDB.Students.First(s => s.StudentNumber == loggedInUser.StudentNumber);
        }

        public async Task<IActionResult> Index()
        {
            Student student = await getLoggedInStudent();
            if(student == null)
            {
                TempData["errorMessage"] = "No student account related";
            }
            return View();
        }

        public IActionResult DisplayCourses()
        {
            return View("Display", courseDB.Courses);
        }

        public async Task<IActionResult> Enrol(string courseCode)
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
            Student student = await getLoggedInStudent();
            if (student == null)
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
                TempData["successMessage"] = $"Enrollment completed! You are enrolled to course {courseCode}";
                return RedirectToAction(nameof(MyCourses));
            }
            return RedirectToAction(nameof(DisplayCourses));
        }

        public async Task<IActionResult> MyCourses()
        {
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "no student data found";
                return RedirectToAction(nameof(DisplayCourses));
            }
            return View(courseDB.Courses.Where(c => c.Students.Contains(student)));
        }

        public async Task<IActionResult> Drop(string courseCode)
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
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "no student data found";
                return RedirectToAction(nameof(DisplayCourses));
            }
            if (student.Courses.Contains(course))
            {
                CourseStudent csdb = course.DeleteStudent(student);
                studentDB.SaveStudent(student);
                csDB.DeleteCourseStudents(csdb);
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"drop completed! You are no longer have course {courseCode}";
                return RedirectToAction(nameof(MyCourses));
            }
            return RedirectToAction(nameof(MyCourses));
        }

        public IActionResult ReviewForm(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "You have to select a course to add student!";
                return RedirectToAction(nameof(MyCourses));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course";
                return RedirectToAction(nameof(MyCourses));
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