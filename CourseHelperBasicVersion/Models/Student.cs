using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public enum STUDENT_STATUS
    {
        PAID,
        UNPAID,
        PENDING
    }
    public class Student
    {
        public int StudentId { get; set; }
        public long StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int Semester { get; set; }
        public STUDENT_STATUS Status { get; set; }
        public Boolean IsRegistered { get; set; }
        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        private IEnumerable<Course> GetCourses()
        {
            foreach (CourseStudent cs in CourseStudents)
            {
                yield return cs.Course;
            }
        }
        [NotMapped]
        public ICollection<Course> Courses { get { return GetCourses().ToList<Course>(); } }

        //public CourseStudent AddCourse(Course course)
        //{
        //    if (course.Students.Contains(this))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        CourseStudent cs = new CourseStudent(course, this);
        //        course.CourseStudents.Add(cs);
        //        CourseStudents.Add(cs);
        //        return cs;
        //    }
        //}
    }
}
