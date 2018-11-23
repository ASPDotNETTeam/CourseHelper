using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public class CourseStudent
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public CourseStudent(Course course, Student student)
        {
            Course = course;
            Student = student;
            CourseId = course.CourseId;
            StudentId = student.StudentId;
        }

        public CourseStudent() { }
    }
}
