using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Database
{
    public class EFStudentDatabase : IStudentDatabase
    {
        private CourseHelperDBContext dbContext;

        public EFStudentDatabase(CourseHelperDBContext context)
        {
            dbContext = context;
        }

        public IQueryable<Student> Students
        {
            get
            {
                return dbContext.Students
                        .Include(s => s.CourseStudents)
                        .ThenInclude(cs => cs.Course);
            }
        }

        public void SaveStudent(Student student)
        {
            if (student.StudentId == 0)
            {
                dbContext.Students.Add(student);
            }
            else
            {
                Student dbStudent = dbContext.Students.FirstOrDefault(s => s.StudentId == student.StudentId);
                if (dbStudent != null)
                {
                    dbStudent.FirstName = student.FirstName;
                    dbStudent.CourseStudents = student.CourseStudents;
                    dbStudent.IsRegistered = student.IsRegistered;
                    dbStudent.LastName = student.LastName;
                    dbStudent.MiddleName = student.MiddleName;
                    dbStudent.Semester = student.Semester;
                    dbStudent.Status = student.Status;
                    dbStudent.StudentNumber = student.StudentNumber;
                }
            }
            dbContext.SaveChanges();
        }

        public Student DeleteStudent(int studentId)
        {
            Student dbStudent = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (dbStudent != null)
            {
                foreach (CourseStudent cs in dbStudent.CourseStudents)
                {
                    //gets course from DB
                    Course dbCourse = dbContext.Courses.First(c => c == cs.Course);
                    //Remove course link from student
                    dbCourse.CourseStudents.Remove(cs);
                    //Remove the linked CourseStudent from 
                    dbContext.CourseStudents.Remove(cs);
                }
                dbContext.Students.Remove(dbStudent);
                dbContext.SaveChanges();
            }
            return dbStudent;
        }

        public void DeleteStudent(Student student)
        {
            if (student != null)
            {
                foreach (CourseStudent cs in student.CourseStudents)
                {
                    //gets course from DB
                    Course dbCourse = dbContext.Courses.First(c => c == cs.Course);
                    //Remove course link from student
                    dbCourse.CourseStudents.Remove(cs);
                    //Remove the linked CourseStudent from 
                    dbContext.CourseStudents.Remove(cs);
                }
                dbContext.Students.Remove(student);
                dbContext.SaveChanges();
            }
        }
    }
}
