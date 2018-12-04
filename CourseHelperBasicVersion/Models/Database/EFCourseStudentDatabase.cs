using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public class EFCourseStudentDatabase : ICourseStudentDatabase
    {
        private CourseHelperDBContext dbContext;

        public EFCourseStudentDatabase(CourseHelperDBContext context)
        {
            dbContext = context;
        }

        public IQueryable<CourseStudent> CourseStudents
        {
            get { return dbContext.CourseStudents; }
        }
        
        public void AddCourseStudents(CourseStudent courseStudent)
        {
            CourseStudent dbcs = dbContext.CourseStudents.FirstOrDefault(cs => cs.CourseId == courseStudent.CourseId && cs.StudentId == courseStudent.StudentId);
            if (dbcs == null)
            {
                dbContext.CourseStudents.Add(dbcs);
            }
        }
    }
}
