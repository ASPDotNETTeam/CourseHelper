using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CourseHelperBasicVersion.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class GuestController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}