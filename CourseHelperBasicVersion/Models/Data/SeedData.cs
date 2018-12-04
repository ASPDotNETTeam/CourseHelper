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
                addSeedStudents(context);
            }
            context.SaveChanges();
        }

        private static void addSeedCourses(CourseHelperDBContext context)
        {
            context.Courses.AddRange(
                new Course { Code = "BUSN-331", Name = "Business Law", Season = SEASON.FALL, Faculty = "Business", Description = "A study on advanced business law in Canada." },
                new Course { Code = "COMP-106", Name = "Applied Business Software I", Season = SEASON.FALL, Faculty = "Business", Description = "Introduction to various business softwares." },
                new Course { Code = "MKTG-222", Name = "Retailing", Season = SEASON.WINTER, Faculty = "Business", Description = "A look at the logistics of retailing and its impact on the economy." },
                new Course { Code = "ACCT-222", Name = "Management Accounting I", Season = SEASON.FALL, Faculty = "Business", Description = "Introduction to management accounting." },
                new Course { Code = "ECON-205", Name = "Principles of Microeconomics", Season = SEASON.WINTER, Faculty = "Business", Description = "A closer look at the scale of microeconomics." }
                );
        }

        private static void addSeedStudents(CourseHelperDBContext context)
        {
            context.Students.AddRange(
                new Student { StudentNumber = 300589632, FirstName = "Atticus", LastName = "Finch", Semester = 2, Status = STUDENT_STATUS.PAID, IsRegistered = true },
                new Student { StudentNumber = 300896112, FirstName = "Boo", LastName = "Radley", Semester = 3, Status = STUDENT_STATUS.PENDING, IsRegistered = false },
                new Student { StudentNumber = 300589657, FirstName = "Jem", LastName = "Finch", Semester = 2, Status = STUDENT_STATUS.PAID, IsRegistered = true },
                new Student { StudentNumber = 300332225, FirstName = "Maudie", LastName = "Atkinson", Semester = 2, Status = STUDENT_STATUS.PENDING, IsRegistered = true },
                new Student { StudentNumber = 300745442, FirstName = "Dill", LastName = "Harris", Semester = 2, Status = STUDENT_STATUS.UNPAID, IsRegistered = false }
                );
        }
    }
}
