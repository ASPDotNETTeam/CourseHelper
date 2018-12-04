using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public interface ICourseDatabase
    {
        IQueryable<Course> Courses { get; }
        void SaveCourse(Course course);
        Course DeleteCourse(int courseID);
        void DeleteCourse(Course course);
    }
}
