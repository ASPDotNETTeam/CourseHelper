using CourseHelperBasicVersion.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models
{
    public static class SeedData
    {
        public static void Populate(IApplicationBuilder app)
        {
            CourseHelperDBContext context = app.ApplicationServices.GetRequiredService<CourseHelperDBContext>();
            context.Database.Migrate();
            if (!context.Courses?.Any() ?? true)
            {
                addSeedCourses(context);
            }
            if (!context.Students?.Any() ?? true)
            {
                addSeedStudents(context);
            }
            if (!context.Reviews?.Any() ?? true) {
                addSeedReviews(context);
            }
            context.SaveChanges();
        }

        private static void addSeedCourses(CourseHelperDBContext context)
        {
            context.Courses.AddRange(
                new Course { Code = "BUSN-331", Name = "Business Law", Season = SEASON.FALL, Faculty = "Business", MaxSize = 30, Description = "A study on advanced business law in Canada." },
                new Course { Code = "COMP-106", Name = "Applied Business Software I", Season = SEASON.FALL, Faculty = "Business", MaxSize = 29, Description = "Introduction to various business softwares." },
                new Course { Code = "MKTG-222", Name = "Retailing", Season = SEASON.WINTER, Faculty = "Business", MaxSize = 31, Description = "A look at the logistics of retailing and its impact on the economy." },
                new Course { Code = "ACCT-222", Name = "Management Accounting I", Season = SEASON.FALL, Faculty = "Business", MaxSize = 33, Description = "Introduction to management accounting." },
                new Course { Code = "ECON-205", Name = "Principles of Microeconomics", Season = SEASON.WINTER, Faculty = "Business", MaxSize = 20, Description = "A closer look at the scale of microeconomics." }
                );
        }

        private static void addSeedStudents(CourseHelperDBContext context)
        {
            context.Students.AddRange(
                new Student { StudentNumber = 300589632, FirstName = "Atticus", LastName = "Finch", Semester = 2, Status = STUDENT_STATUS.PAID, IsRegistered = true },
                new Student { StudentNumber = 300896112, FirstName = "Boo", LastName = "Radley", Semester = 3, Status = STUDENT_STATUS.PENDING, IsRegistered = false },
                new Student { StudentNumber = 300589657, FirstName = "Jem", LastName = "Finch", Semester = 2, Status = STUDENT_STATUS.PAID, IsRegistered = true },
                new Student { StudentNumber = 300332225, FirstName = "Maudie", LastName = "Atkinson", Semester = 2, Status = STUDENT_STATUS.PENDING, IsRegistered = true },
                new Student { StudentNumber = 300745442, FirstName = "Dill", LastName = "Harris", Semester = 2, Status = STUDENT_STATUS.UNPAID, IsRegistered = false },
                new Student { StudentNumber = 123456788, FirstName = "Student", LastName = "Stoodent", Semester = 1, Status = STUDENT_STATUS.PAID, IsRegistered = true }
                );
        }

        private static void addSeedReviews(CourseHelperDBContext context)
        {
            context.Reviews.AddRange(
                new Review { CourseCode= "BUSN-331", CourseName= "Business Law", Semester="FALL2018", Comment="This is a great course!", CreatorName="Hang Li", CreateTime=DateTime.Now },
                new Review { CourseCode = "COMP-106", CourseName = "Applied Business Software I", Semester = "FALL2019", Comment = "bad course", CreatorName = "Anonymous", CreateTime = DateTime.Now }
            );
        }
    }
}
