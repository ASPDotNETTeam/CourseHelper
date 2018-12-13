using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public class EFCourseDatabase : ICourseDatabase
    {
        private CourseHelperDBContext dbContext;
        public EFCourseDatabase(CourseHelperDBContext context)
        {
            dbContext = context;
        }

        public IQueryable<Course> Courses
        {
            get
            {
                return dbContext.Courses
                        .Include(c => c.CourseStudents)
                        .ThenInclude(cs => cs.Student);
            }
        }

        public void SaveCourse(Course course)
        {
            if (course.CourseId == 0)
            {
                dbContext.Courses.Add(course);
            }
            else
            {
                Course dbCourse = dbContext.Courses.FirstOrDefault(c => c.CourseId == course.CourseId);
                if (dbCourse != null)
                {
                    dbCourse.Code = course.Code;
                    dbCourse.Description = course.Description;
                    dbCourse.Faculty = course.Faculty;
                    dbCourse.Name = course.Name;
                    dbCourse.Season = course.Season;
                    dbCourse.MaxSize = course.MaxSize;
                    dbCourse.CourseStudents = course.CourseStudents;
                }
            }
            dbContext.SaveChanges();
        }

        public Course DeleteCourse(int courseID)
        {
            Course dbCourse = dbContext.Courses.FirstOrDefault(c => c.CourseId == courseID);
            if (dbCourse != null)
            {
                foreach (CourseStudent cs in dbCourse.CourseStudents)
                {
                    //gets student from DB
                    Student dbStudent = dbContext.Students.First(s => s == cs.Student);
                    //Remove course link from student
                    dbStudent.CourseStudents.Remove(cs);
                    //Remove the linked CourseStudent from 
                    dbContext.CourseStudents.Remove(cs);
                }
                dbContext.Courses.Remove(dbCourse);
                dbContext.SaveChanges();
            }
            return dbCourse;
        }

        public void DeleteCourse(Course course)
        {
            if (course != null)
            {
                foreach (CourseStudent cs in course.CourseStudents)
                {
                    //gets student from DB
                    Student dbStudent = dbContext.Students.First(s => s == cs.Student);
                    //Remove course link from student
                    dbStudent.CourseStudents.Remove(cs);
                    //Remove the linked CourseStudent from 
                    dbContext.CourseStudents.Remove(cs);
                }
                dbContext.Courses.Remove(course);
                dbContext.SaveChanges();
            }
        }
    }
}
