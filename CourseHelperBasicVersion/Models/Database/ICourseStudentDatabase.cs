using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public interface ICourseStudentDatabase
    {
        IQueryable<CourseStudent> CourseStudents { get; }
        void AddCourseStudents(CourseStudent courseStudent);
        void DeleteCourseStudents(CourseStudent courseStudent);
    }
}
