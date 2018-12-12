using CourseHelperBasicVersion.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseHelperBasicVersion.Models.Component
{
    public class IdentitySummary : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            if (HttpContext.User != null)
            {
                return View("IdentityInfo", new IdentityInfo() { Name = HttpContext.User.Identity.Name});
            } 
            else
            {
                return View("IdentityInfo", new IdentityInfo());
            }
        }
    }
}
