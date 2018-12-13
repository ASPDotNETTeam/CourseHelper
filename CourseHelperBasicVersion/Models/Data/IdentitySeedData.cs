using CourseHelperBasicVersion.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Data
{
    public static class IdentitySeedData
    {
        public static async void Populate(IApplicationBuilder app)
        {
            AppIdentityDBContext context = app.ApplicationServices.GetRequiredService<AppIdentityDBContext>();
            context.Database.EnsureCreated();
            UserManager<CourseHelperUser> userManager = app.ApplicationServices.GetRequiredService<UserManager<CourseHelperUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityRole adminRole = await roleManager.FindByNameAsync("Admin");
            IdentityRole facultyRole = await roleManager.FindByNameAsync("Faculty");
            IdentityRole studentRole = await roleManager.FindByNameAsync("Student");
            if (adminRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (facultyRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Faculty"));
            }
            if (studentRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }
            
            CourseHelperUser userAdmin = await userManager.FindByNameAsync("admin");
            if (userAdmin == null) {
                userAdmin = new CourseHelperUser() { UserName = "admin", Role = "Admin" };
                await userManager.CreateAsync(userAdmin, "password");
                await userManager.AddToRoleAsync(userAdmin, "Admin");
                await userManager.AddToRoleAsync(userAdmin, "Faculty");
            }

            CourseHelperUser userFaculty = await userManager.FindByNameAsync("faculty");
            if (userFaculty == null)
            {
                userFaculty = new CourseHelperUser() { UserName = "faculty", Role ="Faculty" };
                await userManager.CreateAsync(userFaculty, "password");
                await userManager.AddToRoleAsync(userFaculty, "Faculty");
                
            }

            CourseHelperUser userStudent = await userManager.FindByNameAsync("student");
            if (userStudent == null)
            {
                userStudent = new CourseHelperUser() { UserName = "student", StudentNumber = 123456788, Role = "Student" };
                await userManager.CreateAsync(userStudent, "password");
                await userManager.AddToRoleAsync(userStudent, "Student");
            }
        }
    }
}
