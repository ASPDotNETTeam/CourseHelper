using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelperBasicVersion.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ReviewController : Controller
    {
        private ICourseDatabase courseDB;
        private IStudentDatabase studentDB;
        private ICourseStudentDatabase csDB;
        private IReviewDatabase reviewDB;
        private UserManager<CourseHelperUser> userManager;

        public ReviewController(ICourseDatabase courseDB, IStudentDatabase studentDB, ICourseStudentDatabase csDB, IReviewDatabase reviewDB, UserManager<CourseHelperUser> userManager)
        {
            this.courseDB = courseDB;
            this.studentDB = studentDB;
            this.csDB = csDB;
            this.reviewDB = reviewDB;
            this.userManager = userManager;
        }

        public IActionResult Create(string courseCode)
        {
            if (courseCode == null)
            {
                TempData["errorMessage"] = "Please select a course to review.";
                return RedirectToAction("EnrolledCoursesList", "Student");
            }
            Course course = courseDB.Courses.FirstOrDefault(c => c.Code.ToUpper() == courseCode.ToUpper());
            if (course == null)
            {
                TempData["errorMessage"] = "Please select a valid course.";
                return RedirectToAction("EnrolledCoursesList", "Student");
            }
            Review review = new Review() { CourseCode = courseCode, CourseName = course.Name };
            return View(review);
        }

        public IActionResult List()
        {
            return View("List", reviewDB.Reviews);
        }

        [HttpPost]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
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
                TempData["successMessage"] = "Review successfully created.";
                return RedirectToAction(nameof(List));
            }
            else
            {
                return View("Create", review);
            }
        }
    }
}