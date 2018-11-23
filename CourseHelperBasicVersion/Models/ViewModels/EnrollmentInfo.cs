using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.ViewModels
{
    public class EnrollmentInfo
    {
        public Course Course { get; set; }
        public IEnumerable<Student> EnrolledStudents { get; set; }
        public IEnumerable<Student> UnenrolledStudents { get; set; }
        public IList<Student> EnrollQueue { get; set; }
    }
}