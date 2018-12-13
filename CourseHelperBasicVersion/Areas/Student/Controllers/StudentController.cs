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

        public async Task<IActionResult> Index()
        {
            Student student = await getLoggedInStudent();
            if(student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
            }
            return View("Index");
        }

        public async Task<IActionResult> List()
        {
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
            }
            return View("List", courseDB.Courses);
        }

        public async Task<IActionResult> Enroll(string courseCode)
        {
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
            }
            ViewBag.Student = student;
            if (courseCode == null)
            {
                TempData["errorMessage"] = "Please select a course.";
                return RedirectToAction(nameof(List));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course.";
                return RedirectToAction(nameof(List));
            }
            return View("Enroll", course);
        }

        public async Task<IActionResult> UnEnroll(string courseCode)
        {
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
            }
            ViewBag.Student = student;
            if (courseCode == null)
            {
                TempData["errorMessage"] = "Please select a course.";
                return RedirectToAction(nameof(EnrolledCoursesList));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course.";
                return RedirectToAction(nameof(List));
            }
            return View("Enroll", course);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int CourseId)
        {
            if (CourseId == 0)
            {
                TempData["errorMessage"] = "No course was selected. Please try again";
                return RedirectToAction(nameof(List));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.CourseId == CourseId);
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course.";
                return RedirectToAction(nameof(List));
            }
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
                return RedirectToAction(nameof(List));
            }
            if (course.Students.Contains(student))
            {
                TempData["errorMessage"] = "You are already enrolled this course.";
                return View("Enroll", course);
            }
            else if (course.Students.Count >= course.MaxSize)
            {
                TempData["errorMessage"] = "This course is currently full. Please try again when there is space.";
                return View("Enroll", course);
            }
            else if (ModelState.IsValid)
            {
                CourseStudent csdb = course.AddStudent(student);
                studentDB.SaveStudent(student);
                csDB.AddCourseStudents(csdb);
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"Successfully enrolled in {course.Code}: {course.Name}.";
                return RedirectToAction(nameof(EnrolledCoursesList));
            }
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> EnrolledCoursesList()
        {
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
                return RedirectToAction(nameof(List));
            }
            return View("EnrolledCourses", courseDB.Courses.Where(c => c.Students.Contains(student)));
        }

        [HttpPost]
        public async Task<IActionResult> Drop(int CourseId)
        {
            if (CourseId == 0)
            {
                TempData["errorMessage"] = "No course selected. Please try again.";
                return RedirectToAction(nameof(EnrolledCoursesList));
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.CourseId == CourseId);
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course.";
                return RedirectToAction(nameof(EnrolledCoursesList));
            }
            Student student = await getLoggedInStudent();
            if (student == null)
            {
                TempData["errorMessage"] = "No student registered with this account. Please contact customer support.";
                return RedirectToAction(nameof(List));
            }
            if (student.Courses.Contains(course))
            {
                CourseStudent csdb = course.DeleteStudent(student);
                studentDB.SaveStudent(student);
                csDB.DeleteCourseStudents(csdb);
                courseDB.SaveCourse(course);
                TempData["successMessage"] = $"Successfully dropped course {course.Code}: {course.Name}.";
                return RedirectToAction(nameof(EnrolledCoursesList));
            }
            return View("Enroll", course);
        }

        [NonAction]
        private async Task<Student> getLoggedInStudent()
        {
            CourseHelperUser loggedInUser = await userManager.GetUserAsync(HttpContext.User);
            return studentDB.Students.First(s => s.StudentNumber == loggedInUser.StudentNumber);
        }
    }
}