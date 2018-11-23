using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHelperBasicVersion
{
    public class Startup { 

        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
    
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds database connection to services
            services.AddDbContext<CourseHelperDBContext>(options => options.UseSqlServer(Configuration["Data:CourseHelper:ConnectionString"]));
            // Setting the lifetime of table access
            services.AddTransient<ICourseDatabase, EFCourseDatabase>();
            services.AddTransient<IStudentDatabase, EFStudentDatabase>();
            services.AddTransient<ICourseStudentDatabase, EFCourseStudentDatabase>();
            services.AddMvc();
            //services.AddSingleton<CourseRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "",
                    template: "Home/Manage/{courseCode}",
                    defaults: new { controller = "Home", action = "Manage"}
                    );
                routes.MapRoute(
                    name: "",
                    template: "Home/Edit/{courseCode}",
                    defaults: new { controller = "Home", action = "Edit" }
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}"
                    );
            });
            SeedData.Populate(app);
        }
    }
}
